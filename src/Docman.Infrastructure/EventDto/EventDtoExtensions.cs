using System;
using Docman.Domain;
using Docman.Domain.DocumentAggregate;
using Docman.Domain.Events;
using LanguageExt;

namespace Docman.Infrastructure.EventDto
{
    public static class EventDtoExtensions
    {
        public static DocumentCreatedEventDto ToDto(this DocumentCreatedEvent @event) =>
            new DocumentCreatedEventDto
            {
                Id = @event.EntityId.ToString(),
                Number = @event.Number.Value,
                Description = @event.Description.Value,
                TimeStamp = @event.TimeStamp
            };

        public static DocumentApprovedEventDto ToDto(this DocumentApprovedEvent @event) =>
            new DocumentApprovedEventDto
            {
                Id = @event.EntityId.ToString(),
                TimeStamp = @event.TimeStamp
            };

        public static Validation<Error, Event> ToEvent(this DocumentCreatedEventDto dto) =>
            DocumentNumber.Create(dto.Number)
                .Bind(num => DocumentDescription.Create(dto.Description)
                    .Map(desc => (Event) new DocumentCreatedEvent(Guid.Parse(dto.Id), num, desc)));

        public static Validation<Error, Event> ToEvent(this DocumentApprovedEventDto dto) =>
            new DocumentApprovedEvent(Guid.Parse(dto.Id));
    }
}