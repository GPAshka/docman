using System;
using Docman.Domain.DocumentAggregate;

namespace Docman.Domain.Events
{
    public class DocumentCreatedEvent : Event
    {
        public DocumentCreatedEvent(Guid entityId, DocumentNumber number, DocumentDescription description) :
            base(entityId)
        {
            Number = number;
            Description = description;
        }

        public DocumentNumber Number { get; }
        public DocumentDescription Description { get; }
    }
}