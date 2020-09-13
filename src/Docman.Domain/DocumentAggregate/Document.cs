using System;
using Docman.Domain.Events;
using LanguageExt;

namespace Docman.Domain.DocumentAggregate
{
    public class Document : Record<Document>
    {
        public Guid Id { get; }
        public DocumentNumber Number { get; }

        public DocumentDescription Description { get; }
        public DocumentStatus Status { get; }

        internal Document(Guid id, DocumentNumber number, DocumentDescription description,
            DocumentStatus status = DocumentStatus.Created)
        {
            Id = id;
            Number = number;
            Description = description;
            Status = status;
        }

        public Document WithStatus(DocumentStatus status) => new Document(Id, Number, Description, status);

        public Validation<Error, (Document Document, Event Event)> Approve()
        {
            if (Status != DocumentStatus.Created)
                return new Error("Document should have Created status");
            
            var evt = new DocumentApprovedEvent(Id);
            var newState = this.Apply(evt);

            return (newState, evt as Event);
        }

        /*public static Validation<Error, (Document Document, Event Event)> Create(Guid id, string number, string description)
        {
            return DocumentNumber.Create(number)
                .Bind(num => DocumentDescription.Create(description)
                    .Map(desc => new Document(id, num, desc)))
                .Map(d => (d, new DocumentCreatedEvent(d.Id, d.Number, d.Description) as Event));
        }*/
    }
}