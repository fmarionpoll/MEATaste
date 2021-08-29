using MEATaste.DataMEA.Models;

namespace MEATaste.Infrastructure
{
    public class ApplicationState
    {
        public StateProperty<MeaExperiment> CurrentMeaExperiment { get; }
        public StateProperty<Electrode> SelectedElectrode { get; }

        public ApplicationState(StatePropertyFactory statePropertyFactory)
        {
            CurrentMeaExperiment = statePropertyFactory.Create<MeaExperiment>(null, EventType.CurrentExperimentChanged);
            SelectedElectrode = statePropertyFactory.Create<Electrode>(null, EventType.SelectedElectrodeChanged);
        }
    }

    public class StatePropertyFactory
    {
        private readonly IEventRaiser eventRaiser;

        public StatePropertyFactory(IEventRaiser eventRaiser) => this.eventRaiser = eventRaiser;

        public StateProperty<T> Create<T>(T value = default, EventType? eventType = null) =>
            new(eventRaiser, value, eventType);
    }

    public class StateProperty<T>
    {
        private T property;
        private readonly EventType? eventType;
        private readonly IEventRaiser eventRaiser;

        public StateProperty(IEventRaiser eventRaiser, T value, EventType? eventType)
        {
            property = value;
            this.eventType = eventType;
            this.eventRaiser = eventRaiser;
        }

        public T Get() => property;

        public void Set(T value)
        {
            property = value;
            if(eventType != null)
                eventRaiser.Raise(eventType.Value);
        }
    }
}
