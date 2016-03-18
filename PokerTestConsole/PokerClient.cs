using System;
using PokerPolker;

namespace PokerTestConsole
{
    public class PokerClient
    {
        readonly Deck deck;
        protected readonly IEventBroker Events;
        protected readonly Guid ClientId = Guid.NewGuid();
        protected Player player;
        bool wasSmall,wasBig,hasBet;
        public PokerClient(Deck deck, IEventBroker events)
        {
            this.deck = deck;
            this.Events = events;
            this.Events.Publish(new SeatRequested(ClientId));
            this.Events.Subscribe<SeatReserved>(e => e.ClientId == ClientId, ConfirmPlayerName);
            this.Events.Subscribe<PlayerAdded>(e => e.ClientId == ClientId, PlayerNameConfimred);
            this.Events.Subscribe<ReservationNotFound>(e => e.ClientId == ClientId, BadReservanio);
            this.Events.Subscribe<PlayerNameRefused>(e => e.ClientId == ClientId, SetUserNameAgain);
            this.Events.Subscribe<AllPlayersReady>(CutDeck);
            this.Events.Subscribe<RoundStarted>(RoundStartedUp);
            this.Events.Subscribe<PlayerCardsDealt>(PlayerCardsOut);
        }

        void PlayerCardsOut(PlayerCardsDealt obj)
        {
            if (wasSmall)// or current bet is to little, or we can check
            {
                Events.Publish(new PlayerCalled(10, player));
            }
            else if (wasBig)
            {
                Events.Publish(new PlayerChecked(player));
            }
            else
            {
                Events.Publish(new PlayerCalled(20, player));
            }
        }

        void RoundStartedUp(RoundStarted obj)
        {
            if (obj.players.IndexOf(player) == 0)
            {
                Events.Publish(new SmallBlindAdded(10, player));
                wasSmall = true;
            }
            else if (obj.players.IndexOf(player) == 1)
            {
                Events.Publish(new BigBlindAdded(20, player));
                wasBig = true;
            }
        }

        void CutDeck(AllPlayersReady obj)
        {
            Events.Publish(new DealerCardCut(player, deck.PopRandom()));
        }

        protected virtual void PlayerNameConfimred(PlayerAdded obj)
        {
            player = obj.Player;
            Console.WriteLine("Press any key when ready.");
            Console.ReadKey();
            Events.Publish(new PlayerReady(player));
        }

        protected virtual void BadReservanio(ReservationNotFound obj)
        {
            Console.WriteLine("server seems to have forgotten about us :(, please restart and try again");
        }

        protected virtual void SetUserNameAgain(PlayerNameRefused obj)
        {
            Console.WriteLine("that name is taken!");
            Console.WriteLine("Please Enter your player name");
            var name = Console.ReadLine();
            Events.Publish(new AddPlayer(ClientId, name));
        }

        protected virtual void ConfirmPlayerName(SeatReserved obj)
        {
            Console.WriteLine($"you are in seat {obj.SeatPosition}");
            Console.WriteLine("Please Enter your player name");
            var name = Console.ReadLine();
            Events.Publish(new AddPlayer(ClientId, name));
        }

        public class BotPokerClient : PokerClient
        {
            readonly string botName;

            public BotPokerClient(Deck deck, IEventBroker events, string botName) : base(deck, events)
            {
                this.botName = botName;
            }

            protected override void PlayerNameConfimred(PlayerAdded obj)
            {
                player = obj.Player;
                Events.Publish(new PlayerReady(obj.Player));
            }
            protected override void BadReservanio(ReservationNotFound obj) { }
            protected override void ConfirmPlayerName(SeatReserved obj) => Events.Publish(new AddPlayer(ClientId, botName));
            protected override void SetUserNameAgain(PlayerNameRefused obj) => Events.Publish(new AddPlayer(ClientId, botName));
        }

    }
}