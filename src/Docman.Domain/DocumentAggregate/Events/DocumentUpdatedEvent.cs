using System;
using LanguageExt;

namespace Docman.Domain.DocumentAggregate.Events
{
    public class DocumentUpdatedEvent : Event
    {
        public DocumentNumber Number { get; }
        public Option<DocumentDescription> Description { get; }

        public DocumentUpdatedEvent(DocumentId entityId, DocumentNumber number, Option<DocumentDescription> description,
            DateTime timeStamp) : base(entityId.Value, timeStamp)
        {
            Number = number;
            Description = description;
        }

        public static Validation<Error, DocumentUpdatedEvent> Create(Guid documentId, string number,
            string description) =>
            DocumentNumber.Create(number)
                .Bind(num => DocumentDescription.Create(description)
                    .Map(desc => new DocumentUpdatedEvent(new DocumentId(documentId), num, desc, DateTime.UtcNow)));
    }
}