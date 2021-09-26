using System;
using System.Collections.Generic;


namespace MEATaste.Infrastructure
{
    public class EventBus : IEventSubscriber, IEventRaiser
    {
        private readonly List<EventSubscriber> subscribers = new();

        public void Raise(EventType eventType) => subscribers.Iter(x => x.Action());

        public void Subscribe(EventType eventType, Action action) => subscribers.Add(new EventSubscriber(eventType, action));
    }
}
