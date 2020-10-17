using System;
using System.Threading.Tasks;
using Docman.API.Application.Dto;
using Docman.API.Extensions;
using Docman.Domain;

namespace Docman.API.Application.Helpers
{
    public static class Functions
    {
        public static Action<Func<EventDto, Task>, Func<EventDto, Task>, Event> SaveAndPublish =>
            async (saveEvent, publishEvent, @event) =>
            {
                var eventDto = @event.ToDto();
                await saveEvent(eventDto);
                await publishEvent(eventDto);
            };
    }
}