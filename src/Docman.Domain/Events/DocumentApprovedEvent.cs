using System;

namespace Docman.Domain.Events
{
    public class DocumentApprovedEvent : Event
    {
        public DocumentApprovedEvent(Guid entityId) : base(entityId)
        {
        }
    }
}