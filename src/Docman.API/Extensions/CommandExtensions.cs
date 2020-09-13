using Docman.API.Commands;
using Docman.Domain;
using Docman.Domain.DocumentAggregate;
using Docman.Domain.Events;
using LanguageExt;

namespace Docman.API.Extensions
{
    public static class CommandExtensions
    {
        public static Validation<Error, DocumentCreatedEvent> ToEvent(this CreateDocumentCommand command) =>
            DocumentNumber.Create(command.Number)
                .Bind(num => DocumentDescription.Create(command.Description)
                    .Map(desc => new DocumentCreatedEvent(command.DocumentId, num, desc)));
    }
}