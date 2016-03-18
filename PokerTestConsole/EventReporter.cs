using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using PokerPolker;

namespace PokerTestConsole
{
    class EventReporter
    {
        public static void WriteEventsToConsole(Subject<IEvent> domainEvents)
        {
            var timer = Observable.Interval(TimeSpan.FromSeconds(.3));
            var staggeredEvents = domainEvents.Zip(timer, (x, y) => x);

            staggeredEvents.OfType<PlayerReady>().Subscribe(EventReporter.Ready);
            staggeredEvents.OfType<DealerCardCut>().Subscribe(EventReporter.PlayerCut);
            staggeredEvents.OfType<DealerChosen>().Subscribe(EventReporter.DealerChoosed);
            staggeredEvents.OfType<RoundStarted>().Subscribe(EventReporter.NotifyBigAndSmallBlinds);
            staggeredEvents.OfType<SmallBlindAdded>().Subscribe(EventReporter.SmallBlindAdded);
            staggeredEvents.OfType<BigBlindAdded>().Subscribe(EventReporter.BigBlindAdded);
            staggeredEvents.OfType<PlayerCardsDealt>().Subscribe(EventReporter.PlayerCardsDealt);
            staggeredEvents.OfType<PlayerChecked>().Subscribe(EventReporter.Call);
            staggeredEvents.OfType<PlayerCalled>().Subscribe(EventReporter.Check);
        }
        static int eventCount = 0;
        public static void WriteLine(string s)
        {
            Interlocked.Increment(ref eventCount);
            Console.SetCursorPosition(Console.WindowWidth / 2, eventCount % Console.WindowHeight);
            Console.WriteLine(s);
        }

        public static Action<PlayerReady> Ready = 
            p => WriteLine(p.player.Name + " Ready");
        public static Action<DealerCardCut> PlayerCut = 
            c => 
            WriteLine(c.player + " cut " + c.card);
        public static Action<DealerChosen> DealerChoosed = 
            c => WriteLine(c.dealer + " To Deal");
        public static Action<PlayerCardsDealt> PlayerCardsDealt = 
            p => WriteLine("Player Cards Dealt");
        public static Action<SmallBlindAdded> SmallBlindAdded = 
            p => WriteLine($"{p.player.Name} added {p.v} small blind ");
        public static Action<BigBlindAdded> BigBlindAdded = 
            p => WriteLine($"{p.player.Name } added {p.v} big blind ");
        public static Action<RoundStarted> NotifyBigAndSmallBlinds 
            = p => {
                       WriteLine(p.players[0] + " is small");
                       WriteLine(p.players[1] + " is big");
            };
        public static Action<PlayerChecked> Call =
            p => WriteLine(p.Player + " checked");
        public static Action<PlayerCalled> Check =
            p => WriteLine($"{p.Player} called {p.v}");
    }
}