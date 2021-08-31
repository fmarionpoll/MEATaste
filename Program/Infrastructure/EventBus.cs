using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace MEATaste.Infrastructure
{
    public class EventBus : IEventSubscriber, IEventRaiser
    {
        private readonly List<EventSubscriber> subscribers = new();

        public void Raise(EventType eventType) => subscribers.Iter(x => x.Action());

        public void Subscribe(EventType eventType, Action action) => subscribers.Add(new EventSubscriber(eventType, action));
    }

    internal record EventSubscriber(EventType EventType, Action Action);

    public interface IEventSubscriber
    {
        void Subscribe(EventType eventType, Action action);
    }

    public interface IEventRaiser
    {
        void Raise(EventType eventType);
    }

    public enum EventType
    {
        CurrentExperimentChanged = 0,
        SelectedElectrodeChanged = 1,
        ElectrodeRecordLoaded = 2,
        AxesMaxMinChanged = 3,
    }
}
