using System;
using Docman.Domain.DocumentAggregate;

namespace Docman.API.Commands
{
    public class CreateDocumentCommand
    {
        public Guid DocumentId { get; set; }
        public DocumentNumber DocumentNumber { get; set; }
    }
}