namespace MEATaste.Infrastructure
{
    public interface IEventRaiser
    {
        void Raise(EventType eventType);
    }
}