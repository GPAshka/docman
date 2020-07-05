using System;
using LanguageExt;

namespace Docman.Domain.DocumentAggregate
{
    public class Document : Record<Document>
    {
        public Guid Id { get; }
        public DocumentNumber Number { get; }
        public DocumentStatus Status { get; }
        
        public Document(Guid id, DocumentNumber number, DocumentStatus status = DocumentStatus.Created)
        {
            Id = id;
            Number = number;
            Status = status;
        }

        public Document WithStatus(DocumentStatus status) => new Document(Id, Number, status);

    }
}