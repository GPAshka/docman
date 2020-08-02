using System;
using Docman.Domain.Events;
using LanguageExt;

namespace Docman.Domain.DocumentAggregate
{
    public class Document : Record<Document>
    {
        public Guid Id { get; }
        public DocumentNumber Number { get; }
        public DocumentStatus Status { get; }
        
        internal Document(Guid id, DocumentNumber number, DocumentStatus status = DocumentStatus.Created)
        {
            Id = id;
            Number = number;
            Status = status;
        }

        public Document WithStatus(DocumentStatus status) => new Document(Id, Number, status);

        public Validation<Error, (Document Document, Event Event)> Approve()
        {
            if (Status != DocumentStatus.Created)
                return new Error("Document should have Created status");
            
            var evt = new DocumentApprovedEvent(Id);
            var newState = this.Apply(evt);

            return (newState, evt as Event);
        }

        public static Validation<Error, Document> Create(Guid id, string number)
        {
            return DocumentNumber.Create(number)
                .Map(num => new Document(id, num));
        }
    }
}