using System;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using PokerPolker;

namespace PokerTestConsole
{
    public class EventBroker : IEventBroker
    {
        private readonly IScheduler _scheduler;
        private readonly ISubject<IEvent> _subject;
        
        public EventBroker(ISubject<IEvent> subject = null)
        {
            _scheduler = new EventLoopScheduler();
            _subject = subject ?? new Subject<IEvent>();
        }

        public IEventBroker Publish<TEvent>(TEvent @event)
            where TEvent : IEvent
        {
            _subject.OnNext(@event);
            return this;
        }

        public IDisposable Subscribe<TEvent>(Func<TEvent, bool> filter, Action<TEvent> onConsume)
            where TEvent : IEvent
        {
            var subscription = _subject.OfType<TEvent>()
                .Where(filter ?? (x => true))
                .ObserveOn(_scheduler)
                .Subscribe(onConsume);

            return subscription;
        }

        public IDisposable Subscribe<TEvent>(Action<TEvent> onConsume)
            where TEvent : IEvent
        {
            var subscription = _subject.OfType<TEvent>()
                .ObserveOn(_scheduler)
                .Subscribe(onConsume);

            return subscription;
        }

        public void Dispose()
        {
            
        }

        public IDisposable Subscribe(IObserver<IEvent> observer)
        {
            return _subject.Subscribe(observer);
        }
    }
}