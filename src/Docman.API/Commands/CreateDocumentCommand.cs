using System;

namespace Docman.API.Commands
{
    public class CreateDocumentCommand
    {
        public Guid DocumentId { get; }
        public string DocumentNumber { get; }
        
        public CreateDocumentCommand(Guid documentId, string documentNumber)
        {
            DocumentId = documentId;
            DocumentNumber = documentNumber;
        }

        public CreateDocumentCommand WithDocumentId(Guid documentId) =>
            new CreateDocumentCommand(documentId, DocumentNumber);
    }
}