using System;
using LanguageExt;

namespace Docman.Domain.DocumentAggregate.Events
{
    public class DocumentCreatedEvent : Event
    {
        public DocumentCreatedEvent(DocumentId entityId, DocumentNumber number, Option<DocumentDescription> description) :
            base(entityId.Value)
        {
            Number = number;
            Description = description;
        }

        public DocumentCreatedEvent(DocumentId entityId, DocumentNumber number, Option<DocumentDescription> description,
            DateTime timeStamp) : base(entityId.Value, timeStamp)
        {
            Number = number;
            Description = description;
        }

        public DocumentNumber Number { get; }
        public Option<DocumentDescription> Description { get; }
    }
}