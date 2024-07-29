using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using PokerPolker.Events.BettingRounds;

namespace PokerPolker.Model.Rounds
{
    public class BlindsRound : Round
    {
        public Player BettingStartPlayer { get; set; }

        public BlindsRound(Player dealer, IList<Player> playersInSeatingOrder, IEventBroker domainEvents)
            : base(dealer, playersInSeatingOrder, domainEvents)
        {
            BettingStartPlayer = playersInSeatingOrder.Skip(2).First();
            
            // get small blind
            domainEvents.Publish(new SmallBlindExpected(PlayersInDealingOrder().First(), 10));
            Subscribe<SmallBlindAdded>(AddBlind);

            // get big blind
            domainEvents.Publish(new BigBlindExpected(PlayersInDealingOrder().Skip(1).First(), 20));
            Subscribe<BigBlindAdded>(AddBlind);

            // ask for player cards to be dealt, when both big & small blinds are in
            disposables.Add(domainEvents
                .OfType<BigBlindAdded>()
                .Zip(domainEvents.OfType<SmallBlindAdded>().Cast<IBlind>(), (b,r) => b)
                .Take(1)
                .Subscribe(RequestDealerDealPlayerHands));

            // ask for next bet once cards dealt and blinds in
            Subscribe<PlayerCardsDealt>(RequestFirstPlayerBet);
        }

        // prompt for bet once cards dealt
        void RequestFirstPlayerBet(PlayerCardsDealt e)
        {
            // expect bet from next player under the gun (left of big blind)
            currentPlayer = PlayersInDealingOrder().Skip(2).First();
            var lookup = bets.ToLookup(x => x.Player, x => x.Amount);
            PromptPlayerToBet(lookup);
        }

        protected override bool IsBettingComplete(ILookup<Player, int> runningTotals)
        {
            // if last player just went and we're equal, then done
            // 

            // all players must get at least 1 chance to bet

            // if all bets equal and we've all bet at least once (or all checked?)

            // if big blind has not bet, we must give them chance

            // if dealer played and all equal
            //  done

            // if big blind just played and only has 1 bet (their blind, meaning they checked)
            //  done

            // if small blind just matched bigs raise
            //  done

            // if next player is starting player and all is equal, then we're done

            return IsBigBlind(NextPlayer()) &&
                   bets.Any(b => b.Player == NextPlayer()) &&
                   runningTotals.Count == playersInDealingOrder.Count &&
                   runningTotals.Select(x => x.DefaultIfEmpty().Sum()).Distinct().Count() == 1;
        }

        protected override IList<Player> PlayersInPlayingOrder() =>
            playersInSeatingOrder
                .ShiftLeft(playersInSeatingOrder.IndexOf(dealer) + 2);

        private void RequestDealerDealPlayerHands(IBlind e) =>
            domainEvents.Publish(new PlayerCardsRequired(dealer));

        private void AddBlind(IBlind e) => 
            bets.Add(new Bet(e.Player, e.Amount));
    }
}