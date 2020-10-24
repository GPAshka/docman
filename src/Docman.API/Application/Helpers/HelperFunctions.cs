using System;
using System.Threading.Tasks;
using Docman.API.Application.Commands;
using Docman.API.Application.Dto;
using Docman.API.Extensions;
using Docman.Domain;
using Docman.Infrastructure.Repositories;
using LanguageExt;

namespace Docman.API.Application.Helpers
{
    public static class HelperFunctions
    {
        public static Action<Func<EventDto, Task>, Func<EventDto, Task>, Event> SaveAndPublish =>
            async (saveEvent, publishEvent, @event) =>
            {
                var eventDto = @event.ToDto();
                await saveEvent(eventDto);
                await publishEvent(eventDto);
            };

        public static Func<DocumentRepository.GetDocumentByNumber, CreateDocumentCommand, Task<Validation<Error, CreateDocumentCommand>>> ValidateCreateCommand =>
            async (getDocumentByNumber, createCommand) =>
            {
                var document = await getDocumentByNumber(createCommand.Number);
                if (document != null)
                    return new Error($"Document with number '{createCommand.Number}' already exists");

                return createCommand;
            };
    }
}