using System;
using Docman.API.Application.Dto;
using Docman.API.Extensions;
using Docman.Domain;

namespace Docman.API.Application.Helpers
{
    public static class Functions
    {
        public static Action<Action<EventDto>, Action<EventDto>, Event> SaveAndPublish =>
            (saveEvent, publishEvent, @event) =>
            {
                var eventDto = @event.ToDto();
                saveEvent(eventDto);
                publishEvent(eventDto);
            };
    }
}