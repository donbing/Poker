using System;
using System.Collections.Generic;
using System.Reactive.Subjects;
using System.Threading;

namespace PokerPolker
{
    public interface IEvent
    {
    }
    class IOC
    {
        public IOC()
        {
            var store = new HybridStore();//serializer, storage)
            store.RegisterEventSource<AggreggateRoot>();
            // somewhere in the handler we perform atomic update 
            // if there is a concurrency problem, service bus will be 
            // responsible for reapplying changes later 
            store.Update<AggreggateRoot>("project123",
                    e =>
                    {
                        e.AddPlayer("t1");
                        e.AddPlayer("t2");
                        e.AddPlayer("t3");
                    }
            );
        }
    }
    class HybridStore
    {
        IDictionary<Type, Func<object, AggreggateRoot>> _factories = new Dictionary<Type, Func<object, AggreggateRoot>>();
        
        IDictionary<object, AggreggateRoot> aggregates = new Dictionary<object, AggreggateRoot>();

        internal void RegisterEventSource<T>() where T : AggreggateRoot, new()
        {
            _factories[typeof(T)] = (o) => new T();
        }

        internal void Update<T>(string key, Action<T> p)
        {
            throw new NotImplementedException();
        }
    }


    public class AggreggateRoot : ISubject<IEvent, Change>
    {
        Subject<Change> _subject = new Subject<Change>();
        int _version;

        public void OnCompleted()
        {
            _subject.OnCompleted();
        }

        public void OnError(Exception error)
        {
            _subject.OnError(error);
        }

        public void OnNext(IEvent value)
        {
            //EventInvoker.Apply(this, value);
            Interlocked.Increment(ref _version);
            _subject.OnNext(new Change(_version, value));
        }

        public IDisposable Subscribe(IObserver<Change> observer)
        {
            var subscribe = _subject.Subscribe(observer);
            return subscribe;
        }

        internal void AddPlayer(string v)
        {
            throw new NotImplementedException();
        }
    }

    public sealed class Change
    {
        public readonly long Version;
        public readonly IEvent Event;

        public Change(long version, IEvent @event)
        {
            Version = version;
            Event = @event;
        }

        public override string ToString()
        {
            return string.Format("r{0:####}: {1}", Version, Event.ToString());
        }
    }

    internal class GameState
    {
        public static GameState WaitingForPlayers;
        public static GameState CuttingForDealer;
        public static GameState PlayingHands;
    }

    public class HandState
    {
        // blinds added -> deal cards -> 
        public static HandState AwaitingBlinds;
        public static HandState AwaitingPlayerCards;
        public static HandState BettingInPlay;
    }
}