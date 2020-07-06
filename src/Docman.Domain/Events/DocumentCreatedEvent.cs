using System;
using Docman.Domain.DocumentAggregate;

namespace Docman.Domain.Events
{
    public class DocumentCreatedEvent : Event
    {
        public DocumentCreatedEvent(Guid entityId, DocumentNumber number) : base(entityId)
        {
            Number = number;
        }

        public DocumentNumber Number { get; }
    }
}