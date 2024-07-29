using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using PokerPolker.Events;
using PokerPolker.Events.BettingRounds;

namespace PokerPolker.Model.Rounds;

public class Round : IRound
{
    protected IEventBroker domainEvents;
    protected Player dealer;
    protected readonly IList<Player> playersInSeatingOrder;
    protected readonly IList<Player> playersInDealingOrder;
    protected readonly IList<Player> playersInPlayingOrder;
    protected Player currentPlayer;
    protected IList<Bet> bets = new List<Bet>();
    protected CompositeDisposable disposables = new CompositeDisposable();

    public Round(Player dealer, IList<Player> playersInSeatingOrder, IEventBroker domainEvents)
    {
        Console.WriteLine(Environment.NewLine + $"{GetType().Name}");
        this.dealer = dealer;
        this.playersInSeatingOrder = playersInSeatingOrder;
        this.playersInDealingOrder = PlayersInDealingOrder();
        this.playersInPlayingOrder = PlayersInPlayingOrder();
        this.domainEvents = domainEvents;
        currentPlayer = playersInDealingOrder.First();

        Subscribe<PlayerChecked>(e =>
        {
            if (e.Player != currentPlayer)
            {
                domainEvents.Publish(new PlayerPlayedOutOfTurn(e.Player));
                return;
            }

            if (MinBetToStayIn(currentPlayer) is not 0)
            {
                domainEvents.Publish(new PlayerTriedToCheckButHasNotBetEnough(e.Player)); 
                return;
            }

            GotoNextPlayer();
        });

        Subscribe<PlayerCalled>(e =>
        {
            // todo: prevent game advance before these situations
            if (e.Player != currentPlayer)
            {
                domainEvents.Publish(new PlayerPlayedOutOfTurn(e.Player));
                return;
            }

            if (e.Amount != MinBetToStayIn(currentPlayer))
            {
                domainEvents.Publish(new PlayerTriedToCallButHasNotBetEnough(e.Player));
                return;
            }

            bets.Add(new Bet(currentPlayer, e.Amount));
            GotoNextPlayer();
        });

        Subscribe<PlayerFolded>(e =>
        {
            if (e.Player != currentPlayer)
            {
                domainEvents.Publish(new PlayerPlayedOutOfTurn(e.Player));
                return;
            }

            if (MinBetToStayIn(currentPlayer) == 0)
            {
                domainEvents.Publish(new PlayerPlayedOutOfTurn(e.Player));
            }

            GotoNextPlayer(removeCurrent: true);
        });

        Subscribe<PlayerRaised>(e =>
        {
            if (e.Player != currentPlayer)
            {
                domainEvents.Publish(new PlayerPlayedOutOfTurn(e.Player));
                return;
            }
            if (e.Amount < MinBetToStayIn(currentPlayer))
            {
                domainEvents.Publish(new PlayerTriedToRaiseButHasNotBetEnough(e.Player));
                return;
            }

            bets.Add(new Bet(currentPlayer, e.Amount));

            GotoNextPlayer();
        });

        domainEvents.Publish(new RoundStarted(dealer, playersInDealingOrder));
    }

    private int MinBetToStayIn(Player player)
    {
        var runningTotals = bets.ToLookup(b => b.Player, b => b.Amount);
        var currentPlayersTotalBet = runningTotals[player].Sum();
        var highestBet = runningTotals.Max(x => x.Sum() as int?) ?? 0;
        return highestBet - currentPlayersTotalBet;
    }

    protected virtual IList<Player> PlayersInPlayingOrder() => 
        PlayersInDealingOrder();

    private void GotoNextPlayer(bool removeCurrent = false)
    {
        var runningTotals = bets.ToLookup(b => b.Player, b => b.Amount);

        // all payers have bet and bets are equal
        if (IsBettingComplete(runningTotals))
        {
            Dispose();
            currentPlayer = PlayersInDealingOrder().First();
            domainEvents.Publish(new RoundComplete(dealer, bets));
            return;
        }

        var nextPlayer = NextPlayer();

        if (removeCurrent)
        {
            playersInDealingOrder.Remove(currentPlayer);
            playersInPlayingOrder.Remove(currentPlayer);
        }

        if (playersInDealingOrder.Count == 1)
        {
            domainEvents.Publish(new GameWon(playersInDealingOrder.Single(), bets));
            return;
        }

        currentPlayer = nextPlayer;

        PromptPlayerToBet(runningTotals);
    }

    protected void PromptPlayerToBet(ILookup<Player, int> runningTotals)
    {
        var currentPlayersBet = runningTotals[currentPlayer].Sum();
        var maxBet = runningTotals.Max(x => x.DefaultIfEmpty().Sum() as int?) ?? 0;
        var minBetForPlayer = maxBet - currentPlayersBet;
        domainEvents.Publish(new BetExpected(currentPlayer, minBetForPlayer));
    }

    protected virtual bool IsBettingComplete(ILookup<Player, int> runningTotals) =>
            playersInPlayingOrder.Last() == currentPlayer &&
            (!runningTotals.Any() || runningTotals.Select(x => x.Sum()).Distinct().Count() == 1);

    protected void Subscribe<T>(Action<T> action) where T : IEvent => 
        disposables.Add(domainEvents.Subscribe(action));

    public void Dispose() => 
        disposables.Dispose();
    
    // todo: this is fucked. use the field?
    protected IList<Player> PlayersInDealingOrder() => 
        playersInSeatingOrder
            .ShiftLeft(playersInSeatingOrder.IndexOf(dealer) + 1);

    protected Player NextPlayer()
    {
        var nextPlayerIndex = currentPlayer == playersInSeatingOrder.Last() 
            ? 0
            : playersInSeatingOrder.IndexOf(currentPlayer) + 1;

        return playersInSeatingOrder[nextPlayerIndex];
    }

    protected bool IsBigBlind(Player player) =>
        PlayersInDealingOrder().IndexOf(player) == 1;
}