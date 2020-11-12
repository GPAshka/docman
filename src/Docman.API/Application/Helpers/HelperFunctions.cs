using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Docman.API.Application.Dto;
using Docman.API.Extensions;
using Docman.Domain;
using Docman.Domain.DocumentAggregate;
using LanguageExt;

namespace Docman.API.Application.Helpers
{
    public static class HelperFunctions
    {
        public static Func<Func<EventDto, Task>, Func<EventDto, Task>, Event, Task> SaveAndPublish =>
            async (saveEvent, publishEvent, @event) =>
            {
                var eventDto = @event.ToDto();
                await saveEvent(eventDto);
                await publishEvent(eventDto);
            };

        public static
            Func<Func<Guid, Task<Validation<Error, IEnumerable<Event>>>>, Guid, Task<Validation<Error, Document>>>
            GetDocumentFromEvents =>
            async (readEventsFunc, documentId) =>
                await readEventsFunc(documentId)
                    .BindT(events => DocumentHelper.From(events, documentId));
    }
}