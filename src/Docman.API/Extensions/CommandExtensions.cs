using Docman.API.Application.Commands;
using Docman.Domain;
using Docman.Domain.DocumentAggregate;
using Docman.Domain.DocumentAggregate.Events;
using LanguageExt;

namespace Docman.API.Extensions
{
    public static class CommandExtensions
    {
        public static Validation<Error, Event> ToEvent(this CreateDocumentCommand command) =>
            DocumentNumber.Create(command.Number)
                .Bind(num => DocumentDescription.Create(command.Description)
                    .Map(desc => (Event) new DocumentCreatedEvent(new DocumentId(command.Id), num, desc)));
    }
}