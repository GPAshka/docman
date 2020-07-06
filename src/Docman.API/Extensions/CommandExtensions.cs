using System;
using Docman.API.Commands;
using Docman.Domain.Events;

namespace Docman.API.Extensions
{
    public static class CommandExtensions
    {
        public static DocumentCreatedEvent ToEvent(this CreateDocumentCommand command)
            => new DocumentCreatedEvent(command.DocumentId, command.DocumentNumber);

        public static DocumentApprovedEvent ToEvent(this ApproveDocumentCommand command)
            => new DocumentApprovedEvent(command.DocumentId);
    }
}