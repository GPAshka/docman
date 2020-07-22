namespace Docman.Infrastructure.EventStore
{
    public readonly struct EventMetaData
    {
        public EventMetaData(string eventType)
        {
            EventType = eventType;
        }

        public string EventType { get; }
    }
}