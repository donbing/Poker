using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using PokerPolker.Events;
using PokerPolker.Events.BettingRounds;
using PokerPolker.Events.GameStart;

namespace PokerTestConsole
{
    class EventReporter
    {
        public static void WriteEventsToConsole(Subject<IEvent> domainEvents)
        {
            Console.Clear();
            //var timer = Observable.Interval(TimeSpan.FromSeconds(.3));
            WriteLine("Logger Starting.");
            //domainEvents.OfType<PlayerReady>().Subscribe(Ready);
            //domainEvents.OfType<PlayerCutTheDeck>().Subscribe(PlayerCut);
            //domainEvents.OfType<DealerChosen>().Subscribe(DealerChosen);
            //domainEvents.OfType<RoundStarted>().Subscribe(NotifyBigAndSmallBlinds);
            //domainEvents.OfType<SmallBlindAdded>().Subscribe(SmallBlindAdded);
            //domainEvents.OfType<BigBlindAdded>().Subscribe(BigBlindAdded);
            //domainEvents.OfType<PlayerCardsDealt>().Subscribe(PlayerCardsDealt);
            //domainEvents.OfType<PlayerChecked>().Subscribe(Call);
            //domainEvents.OfType<PlayerCalled>().Subscribe(Check);
            //domainEvents.OfType<PlayerFolded>().Subscribe(Folded);
            //domainEvents.OfType<PlayerRaised>().Subscribe(Raised);
            domainEvents.OfType<IEvent>().Subscribe(LogEvent);
            WriteLine("Logger Registered.");
        }

        private static void Log<T>(T p) =>
            WriteLine(p.ToString());

        public static void LogEvent(IEvent evt) =>
            WriteLine(evt.ToString());
        
        public static void WriteLine(string s) => 
            Console.WriteLine(s);

        public static void Ready(PlayerReady p) => 
            WriteLine(p.Player.Name + " Ready.");

        public static void PlayerCut(PlayerCutTheDeck c) => 
            WriteLine(c.Player + " cut " + c.Card);

        public static void DealerChosen(DealerChosen c) => 
            WriteLine(c.Player + " To Deal.");

        public static void PlayerCardsDealt(PlayerCardsDealt p) => 
            WriteLine("Player Cards Dealt.");

        public static void SmallBlindAdded(SmallBlindAdded p) => 
            WriteLine($"{p.Player.Name} added {p.Amount} small blind.");

        // todo: as above ^
        public static Action<BigBlindAdded> BigBlindAdded = 
            p => WriteLine($"{p.Player.Name} added {p.Amount} big blind.");

        public static Action<RoundStarted> NotifyBigAndSmallBlinds 
            = p => WriteLine($"{p.Dealer.Name} dealing to: {string.Join(",", p.Players)} {Environment.NewLine} "
                             + $"{p.Players[0].Name} is small blind, {p.Players[1].Name} is big.");

        public static Action<PlayerChecked> Call =
            p => WriteLine(p.Player + " checked.");

        public static Action<PlayerCalled> Check =
            p => WriteLine($"{p.Player} called {p.Amount}.");

        public static Action<PlayerFolded> Folded =
            p => WriteLine($"{p.Player} folded.");

        public static Action<PlayerRaised> Raised =
            p => WriteLine($"{p.Player} raised {p.Amount}.");
    }
}