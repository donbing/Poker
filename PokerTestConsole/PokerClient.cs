using System;
using PokerPolker.Events.BettingRounds;
using PokerPolker.Events.GameStart;
using PokerPolker.Model;
using PokerPolker.Model.PlayingCards;

namespace PokerTestConsole
{
    public class PokerClient
    {
        readonly Deck deck;

        protected readonly IEventBroker Events;
        protected readonly Guid ClientId = Guid.NewGuid();
        protected Player player;

        public PokerClient(Deck deck, IEventBroker events)
        {
            this.deck = deck;
            Events = events;
            
            // setup
            Events.Subscribe<SeatReserved>(e => e.ClientId == ClientId, ConfirmPlayerName);
            Events.Subscribe<PlayerAddedToGame>(e => e.ClientId == ClientId, PlayerNameConfirmed);
            Events.Subscribe<ReservationNotFound>(e => e.ClientId == ClientId, BadReservation);
            Events.Subscribe<PlayerNameRefused>(e => e.ClientId == ClientId, SetUserNameAgain);

            // dealer selection
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

            if (obj.MinBet == 0)
            {
                if (new Random().Next(1, 3) == 1)
                {
                    Events.Publish(new PlayerRaised(player, 20));
                }
                else
                {
                    Events.Publish(new PlayerChecked(player));
                }
            }
            else if (new Random().Next(1, 3) == 1)
            {
                Events.Publish(new PlayerFolded(player));
            }
            else if (new Random().Next(1, 3) == 1)
            {
                Events.Publish(new PlayerRaised(player, obj.MinBet + 20));
            }
            else
            {
                Events.Publish(new PlayerCalled(player, obj.MinBet));
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

        protected virtual void PlayerNameConfirmed(PlayerAddedToGame obj)
        {
            player = obj.Player;
            Console.WriteLine($"Game joined as {obj.Player.Name}: press any key when ready.");
            //Console.ReadKey();
            Events.Publish(new PlayerReady(player));
        }

        protected virtual void BadReservation(ReservationNotFound obj)
        {
            Console.WriteLine("server seems to have forgotten about us :(, please restart and try again");
        }

        protected virtual void SetUserNameAgain(PlayerNameRefused obj)
        {
            Console.WriteLine("that name is taken!");
            Console.WriteLine("Please Enter your player name");
            var name = "chris";
            Events.Publish(new ClaimSeatReservation(ClientId, name, obj.SeatPosition));
        }

        protected virtual void ConfirmPlayerName(SeatReserved obj)
        {
            Console.WriteLine($"you are in seat {obj.SeatPosition}");
            Console.WriteLine("Please Enter your player name");
            var name = "chris1";
            Events.Publish(new ClaimSeatReservation(ClientId, name, obj.SeatPosition));
        }

        public void JoinGame(PokerGame prog)
        {
            var seatPosition = 1;
            prog.Events.Publish(new SeatRequested(ClientId, seatPosition));
        }
    }
}