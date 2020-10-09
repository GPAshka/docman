using System;
using Docman.API.Dto.Events;
using Docman.Domain;
using Docman.Domain.DocumentAggregate;
using Docman.Domain.Events;
using LanguageExt;

namespace Docman.API.Extensions
{
    public static class EventDtoExtensions
    {
        public static DocumentCreatedEventDto ToDto(this DocumentCreatedEvent @event) =>
            new DocumentCreatedEventDto
            {
                Id = @event.EntityId.ToString(),
                Number = @event.Number.Value,
                Description = @event.Description.Match(Some: d => d.Value, None: string.Empty),
                TimeStamp = @event.TimeStamp
            };

        public static DocumentApprovedEventDto ToDto(this DocumentApprovedEvent @event) =>
            new DocumentApprovedEventDto
            {
                Id = @event.EntityId.ToString(),
                Comment = @event.Comment.Value,
                TimeStamp = @event.TimeStamp
            };

        public static FileAddedEventDto ToDto(this FileAddedEvent @event) =>
            new FileAddedEventDto
            {
                DocumentId = @event.EntityId.ToString(),
                FileId = @event.FileId.ToString(),
                FileName = @event.Name.Value,
                FileDescription = @event.Description.Match(d => d.Value, string.Empty),
                TimeStamp = @event.TimeStamp
            };

        public static Validation<Error, Event> ToEvent(this DocumentCreatedEventDto dto) =>
            DocumentNumber.Create(dto.Number)
                .Bind(num => DocumentDescription.Create(dto.Description)
                    .Map(desc => (Event) new DocumentCreatedEvent(Guid.Parse(dto.Id), num, desc, dto.TimeStamp)));

        public static Validation<Error, Event> ToEvent(this DocumentApprovedEventDto dto) =>
            Comment.Create(dto.Comment)
                .Map(c => (Event) new DocumentApprovedEvent(Guid.Parse(dto.Id), c, dto.TimeStamp));

        public static Validation<Error, Event> ToEvent(this FileAddedEventDto dto) =>
            FileName.Create(dto.FileName)
                .Bind(name => FileDescription.Create(dto.FileDescription)
                    .Map(desc =>
                        (Event) new FileAddedEvent(Guid.Parse(dto.DocumentId), Guid.Parse(dto.FileId), name, desc,
                            dto.TimeStamp)));

    }
}