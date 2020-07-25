using System;
using LanguageExt;
using static LanguageExt.Prelude;

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

        public Validation<Error, Document> Approve()
        {
            return Status != DocumentStatus.Created
                ? Fail<Error, Document>(new Error("Document should have Created status"))
                : WithStatus(DocumentStatus.Approved);
        }

        public static Validation<Error, Document> Create(Guid id, string number)
        {
            return DocumentNumber.Create(number)
                .Map(num => new Document(id, num));
        }
    }
}