using System;
using Docman.Domain.DocumentAggregate;
using LanguageExt;

namespace Docman.Domain.Events
{
    public class DocumentCreatedEvent : Event
    {
        public DocumentCreatedEvent(Guid entityId, DocumentNumber number, Option<DocumentDescription> description) :
            base(entityId)
        {
            Number = number;
            Description = description;
        }

        public DocumentNumber Number { get; }
        public Option<DocumentDescription> Description { get; }
    }
}