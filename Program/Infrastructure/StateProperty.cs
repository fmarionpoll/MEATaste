namespace MEATaste.Infrastructure
{
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