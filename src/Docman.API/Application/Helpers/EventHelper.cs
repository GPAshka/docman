using System;
using System.Threading.Tasks;
using Docman.API.Application.Dto;
using Docman.API.Application.Extensions;
using Docman.Domain;
using LanguageExt;

namespace Docman.API.Application.Helpers
{
    public static class EventHelper
    {
        public static Func<Func<EventDto, Task>, Func<EventDto, Task>, Event, Task<Validation<Error, Event>>>
            SaveAndPublish =>
            async (saveEvent, publishEvent, @event) =>
            {
                var eventDto = @event.ToDto();
                await saveEvent(eventDto);
                await publishEvent(eventDto);
                return Validation<Error, Event>.Success(@event);
            };
    }
}