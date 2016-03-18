using System;

namespace PokerPolker
{
    public interface IEventBroker : IObservable<IEvent>
    {
        void Dispose();
        IEventBroker Publish<TEvent>(TEvent @event) where TEvent : IEvent;
        IDisposable Subscribe<TEvent>(Func<TEvent, bool> filter, Action<TEvent> onConsume) where TEvent : IEvent;
        IDisposable Subscribe<TEvent>(Action<TEvent> onConsume) where TEvent : IEvent;
    }
}