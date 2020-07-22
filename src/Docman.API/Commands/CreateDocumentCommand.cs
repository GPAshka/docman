using System;
using Docman.Domain.DocumentAggregate;

namespace Docman.API.Commands
{
    public class CreateDocumentCommand
    {
        public CreateDocumentCommand(Guid documentId, DocumentNumber documentNumber)
        {
            DocumentId = documentId;
            DocumentNumber = documentNumber;
        }

        public Guid DocumentId { get; }
        public DocumentNumber DocumentNumber { get; }
    }
}