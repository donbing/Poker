
using System;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using PokerPolker.Events;
using PokerPolker.Events.GameStart;
using PokerPolker.Model;

namespace PokerTestConsole.EventStore
{
    public class MemoryEventBroker(ISubject<IEvent> subject = null) : IEventBroker
    {
        private readonly IScheduler scheduler = new EventLoopScheduler();
        private readonly ISubject<IEvent> subject = subject ?? new Subject<IEvent>();

        public IEventBroker Publish<TEvent>(TEvent @event)
            where TEvent : IEvent
        {
            subject.OnNext(@event);
            return this;
        }

        public IDisposable Subscribe<TEvent>(Func<TEvent, bool> filter, Action<TEvent> onConsume)
            where TEvent : IEvent =>
            subject.OfType<TEvent>()
                .Where(filter ?? (x => true))
                .ObserveOn(scheduler)
                .Subscribe(onConsume);

        public IDisposable Subscribe<TEvent>(Action<TEvent> onConsume)
            where TEvent : IEvent =>
            subject.OfType<TEvent>()
                .ObserveOn(scheduler)
                .Subscribe(onConsume);

        public IDisposable Subscribe(IObserver<IEvent> observer)
        {
            return subject.Subscribe(observer);
        }

        public IObservable<TResponse> Publish<TResponse, TRequest>(TRequest seatRequested)
            where TRequest : IRequest<TResponse>, IEvent
        {
            Publish(seatRequested);
            return this.OfType<TResponse>();
        }
    }
}