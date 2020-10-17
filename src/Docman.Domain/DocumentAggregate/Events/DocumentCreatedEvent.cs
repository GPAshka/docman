using System;
using LanguageExt;

namespace Docman.Domain.DocumentAggregate.Events
{
    public class DocumentCreatedEvent : Event
    {
        public DocumentCreatedEvent(Guid entityId, DocumentNumber number, Option<DocumentDescription> description) :
            base(entityId)
        {
            Number = number;
            Description = description;
        }

        public DocumentCreatedEvent(Guid entityId, DocumentNumber number, Option<DocumentDescription> description,
            DateTime timeStamp) :
            base(entityId, timeStamp)
        {
            Number = number;
            Description = description;
        }

        public DocumentNumber Number { get; }
        public Option<DocumentDescription> Description { get; }
    }
}