namespace MEATaste.Infrastructure
{
    public class StatePropertyFactory
    {
        private readonly IEventRaiser eventRaiser;

        public StatePropertyFactory(IEventRaiser eventRaiser) => this.eventRaiser = eventRaiser;

        public StateProperty<T> Create<T>(T value = default, EventType? eventType = null) =>
            new(eventRaiser, value, eventType);
    }
}