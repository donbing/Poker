using System;
using PokerPolker.Events;

namespace PokerPolker.Model
{
    public interface IEventBroker : IObservable<IEvent>
    {
        IEventBroker Publish<TEvent>(TEvent @event) 
            where TEvent : IEvent;

        IDisposable Subscribe<TEvent>(Func<TEvent, bool> filter, Action<TEvent> onConsume) 
            where TEvent : IEvent;

        IDisposable Subscribe<TEvent>(Action<TEvent> onConsume)
            where TEvent : IEvent;
    }
}