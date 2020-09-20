using System;
using Docman.Domain.Events;
using LanguageExt;

namespace Docman.Domain.DocumentAggregate
{
    public class Document : Record<Document>
    {
        public Guid Id { get; }
        public DocumentNumber Number { get; }
        public Option<DocumentDescription> Description { get; }
        public DocumentStatus Status { get; }

        public Document(Guid id, DocumentNumber number, Option<DocumentDescription> description,
            DocumentStatus status = DocumentStatus.Created)
        {
            Id = id;
            Number = number;
            Description = description;
            Status = status;
        }

        public Validation<Error, (Document Document, DocumentApprovedEvent Event)> Approve(string comment)
        {
            if (Status != DocumentStatus.Created)
                return new Error("Document should have Created status");

            return Comment.Create(comment)
                .Map(c => new DocumentApprovedEvent(Id, c))
                .Map(evt => (this.Apply(evt), evt));
        }
    }
}