using PokerPolker.Events.BettingRounds;
using PokerPolker.Events.GameStart;
using PokerPolker.Model;
using PokerPolker.Model.PlayingCards;

namespace PokerPolker.Client;

public class PokerClient
{
    readonly Deck deck;

    protected readonly IEventBroker Events;
    protected readonly Guid ClientId = Guid.NewGuid();
    protected Player player;
    private readonly ConciergeInteraction conciergeInteraction;
    private readonly SeatingInteraction seatingInteraction;

    public PokerClient(Deck deck, IEventBroker events)
    {
        this.deck = deck;
        Events = events;

        // setup
        conciergeInteraction = new ConciergeInteraction(events, ClientId);
        seatingInteraction = SeatingInteraction(events);

        // players cut the deck for a high card to select dealer
        Events.Subscribe<PlayerCutRequired>(CutDeck);

        // first betting round
        Events.Subscribe<PlayerCardsRequired>(DealPlayerCardsIfDealer);
        Events.Subscribe<BetExpected>(SelectBettingStrategy);
        Events.Subscribe<SmallBlindExpected>(AddSmallBlind);
        Events.Subscribe<BigBlindExpected>(AddBigBlind);
        Events.Subscribe<FlopExpected>(DealFlop);
        Events.Subscribe<TurnCardExpected>(DealTurnCard);
        Events.Subscribe<RiverCardExpected>(DealTurnRiverCard);
    }

    protected virtual SeatingInteraction SeatingInteraction(IEventBroker events)
    {
        return new SeatingInteraction(events, ClientId);
    }

    private void DealTurnRiverCard(RiverCardExpected obj)
    {
        if (obj.Dealer == player)
        {
            Events.Publish(new RiverCardDealt());
        }
    }

    private void DealTurnCard(TurnCardExpected obj)
    {
        if (obj.Dealer == player)
        {
            Events.Publish(new TurnCardDealt());
        }
    }

    private void DealFlop(FlopExpected obj)
    {
        if (obj.Dealer == player)
        {
            Events.Publish(new FlopDealt());
        }
    }

    private void AddBigBlind(BigBlindExpected obj)
    {
        if (obj.Player == player)
        {
            Events.Publish(new BigBlindAdded(player, obj.Amount));
        }
    }

    private void AddSmallBlind(SmallBlindExpected obj)
    {
        if (obj.Player == player)
        {
            Events.Publish(new SmallBlindAdded(player, obj.Amount));
        }
    }

    private void SelectBettingStrategy(BetExpected obj)
    {
        if (obj.Player != player)
        {
            return;
        }

        var rnd = new Random().Next(1, 3);

        if (obj.MinBet == 0)
        {
            if (rnd == 1)
            {
                Events.Publish(new PlayerRaised(player, 20));
            }
            else
            {
                Events.Publish(new PlayerChecked(player));
            }
        }
        else
        {
            switch (rnd)
            {
                case 1:
                    Events.Publish(new PlayerFolded(player));
                    break;
                case 2:
                    Events.Publish(new PlayerRaised(player, obj.MinBet + 20));
                    break;
                default:
                    Events.Publish(new PlayerCalled(player, obj.MinBet));
                    break;
            }
        }
    }

    private void DealPlayerCardsIfDealer(PlayerCardsRequired obj)
    {
        if (obj.Dealer == player)
        {
            Events.Publish(new PlayerCardsDealt());
        }
    }

    void CutDeck(PlayerCutRequired @event)
    {
        if (@event.Player == player)
        {
            Events.Publish(new PlayerCutTheDeck(player, deck.PopRandom()));
        }
    }

    public void JoinGame() => 
        conciergeInteraction.RequestAnySeat();

    public void JoinGameAtSeat(int seat) => 
        conciergeInteraction.RequestSpecificSeat(seat);
}