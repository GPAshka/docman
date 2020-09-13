using System;
using Docman.Domain;
using Docman.Domain.DocumentAggregate;
using Docman.Domain.Events;
using LanguageExt;

namespace Docman.API.Commands
{
    public class CreateDocumentCommand
    {
        public Guid DocumentId { get; }
        public string Number { get; }
        public string Description { get; }

        public CreateDocumentCommand(Guid documentId, string number, string description)
        {
            DocumentId = documentId;
            Number = number;
            Description = description;
        }

        public CreateDocumentCommand WithDocumentId(Guid documentId) =>
            new CreateDocumentCommand(documentId, Number, Description);
    }
}