using System;

namespace MEATaste.Infrastructure
{
    public interface IEventSubscriber
    {
        void Subscribe(EventType eventType, Action action);
    }
}