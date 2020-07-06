using System;

namespace Docman.Domain.Events
{
    public class Event
    {
        public Event(Guid entityId) : this(entityId, DateTime.UtcNow)
        {
        }
        
        public Event(Guid entityId, DateTime timeStamp)
        {
            EntityId = entityId;
            TimeStamp = timeStamp;
        }

        public Guid EntityId { get; }
        public DateTime TimeStamp { get; }
    }
}