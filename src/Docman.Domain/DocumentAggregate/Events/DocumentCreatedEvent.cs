using System;
using LanguageExt;

namespace Docman.Domain.DocumentAggregate.Events
{
    public class DocumentCreatedEvent : Event
    {
        public UserId UserId { get; }
        public DocumentNumber Number { get; }
        public Option<DocumentDescription> Description { get; }
        
        public DocumentCreatedEvent(DocumentId entityId, UserId userId, DocumentNumber number, Option<DocumentDescription> description) :
            base(entityId.Value)
        {
            UserId = userId;
            Number = number;
            Description = description;
        }

        public DocumentCreatedEvent(DocumentId entityId, UserId userId, DocumentNumber number, Option<DocumentDescription> description,
            DateTime timeStamp) : base(entityId.Value, timeStamp)
        {
            UserId = userId;
            Number = number;
            Description = description;
        }
    }
}