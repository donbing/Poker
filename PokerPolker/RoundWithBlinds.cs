using System;
using System.Collections.Generic;
using System.Reactive.Linq;

namespace PokerPolker
{
    public class RoundWithBlinds
    {
        class Bet
        {
            public int amount;
            public Player player;

            public Bet(Player player, int v)
            {
                this.player = player;
                this.amount = v;
            }
        }

        private IEventBroker domainEvents;
        Player dealer;

        IList<Bet> bets = new List<Bet>();

        void SubscribeToPlayer<T>(Player player, Action<T> act) where T : IEvent
        {
            domainEvents.Subscribe<T>(p => p.)
        }

        public RoundWithBlinds(Player dealer, IList<Player> playersInOrder, IEventBroker domainEvents)
        {
            this.dealer = dealer;
            this.domainEvents = domainEvents;

            var dealIndex = playersInOrder.IndexOf(dealer);
            var players = playersInOrder.ShiftLeft(dealIndex + 1);

            var smallBlind = domainEvents.Subscribe<SmallBlindAdded>(e => {
                    bets.Add(new Bet(e.player, e.v));
            });
            var bigBlind = domainEvents.OfType<BigBlindAdded>().Subscribe(e => {
                    bets.Add(new Bet(e.player, e.v));
            });
            var cardsDealtToPlayers = domainEvents.OfType<PlayerCardsDealt>().Subscribe(e => {
                    // expect bet from next player
            });

            domainEvents.OfType<PlayerChecked>().Subscribe(e => {
            });
            domainEvents.OfType<PlayerCalled>().Subscribe(e => {
            });
            domainEvents.OfType<PlayerFolded>().Subscribe(e => {
            });

            domainEvents.Publish(new RoundStarted(players));
            domainEvents.Publish(new PlayerCardsDealt());
        }
    }
}