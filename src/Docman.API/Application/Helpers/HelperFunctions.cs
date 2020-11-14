using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Docman.API.Application.Dto;
using Docman.API.Application.Extensions;
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
    }
}