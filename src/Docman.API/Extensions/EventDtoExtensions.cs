using System;
using Docman.API.Application.Dto;
using Docman.API.Application.Dto.Events;
using Docman.Domain;
using Docman.Domain.DocumentAggregate;
using Docman.Domain.DocumentAggregate.Events;
using LanguageExt;

namespace Docman.API.Extensions
{
    public static class EventDtoExtensions
    {
        public static EventDto ToDto(this Event @event) =>
            @event switch
            {
                DocumentCreatedEvent createdEvent => createdEvent.ToDto(),
                DocumentApprovedEvent approvedEvent => approvedEvent.ToDto(),
                FileAddedEvent fileAddedEvent => fileAddedEvent.ToDto(),
                DocumentSentForApprovalEvent sentForApprovalEvent => sentForApprovalEvent.ToDto(),
                DocumentRejectedEvent documentRejectedEvent => documentRejectedEvent.ToDto(),
                _ => throw new ArgumentOutOfRangeException(nameof(@event))    //TODO
            };

        private static DocumentCreatedEventDto ToDto(this DocumentCreatedEvent @event) =>
            new DocumentCreatedEventDto
            {
                Id = @event.EntityId,
                Number = @event.Number.Value,
                Description = @event.Description.Match(Some: d => d.Value, None: string.Empty),
                TimeStamp = @event.TimeStamp
            };

        private static DocumentApprovedEventDto ToDto(this DocumentApprovedEvent @event) =>
            new DocumentApprovedEventDto
            {
                Id = @event.EntityId,
                Comment = @event.Comment.Value,
                TimeStamp = @event.TimeStamp
            };

        private static FileAddedEventDto ToDto(this FileAddedEvent @event) =>
            new FileAddedEventDto
            {
                Id = @event.EntityId,
                FileId = @event.FileId.ToString(),
                FileName = @event.Name.Value,
                FileDescription = @event.Description.Match(d => d.Value, string.Empty),
                TimeStamp = @event.TimeStamp
            };

        private static DocumentSentForApprovalEventDto ToDto(this DocumentSentForApprovalEvent @event) =>
            new DocumentSentForApprovalEventDto
            {
                Id = @event.EntityId,
                TimeStamp = @event.TimeStamp
            };

        private static DocumentRejectedEventDto ToDto(this DocumentRejectedEvent @event) =>
            new DocumentRejectedEventDto
            {
                Id = @event.EntityId,
                Reason = @event.Reason.Value,
                TimeStamp = @event.TimeStamp
            };

        public static Validation<Error, Event> ToEvent(this DocumentCreatedEventDto dto) =>
            DocumentNumber.Create(dto.Number)
                .Bind(num => DocumentDescription.Create(dto.Description)
                    .Map(desc => (Event) new DocumentCreatedEvent(new DocumentId(dto.Id), num, desc, dto.TimeStamp)));

        public static Validation<Error, Event> ToEvent(this DocumentApprovedEventDto dto) =>
            Comment.Create(dto.Comment)
                .Map(c => (Event) new DocumentApprovedEvent(new DocumentId(dto.Id), c, dto.TimeStamp));

        public static Validation<Error, Event> ToEvent(this FileAddedEventDto dto) =>
            FileName.Create(dto.FileName)
                .Bind(name => FileDescription.Create(dto.FileDescription)
                    .Map(desc =>
                        (Event) new FileAddedEvent(new DocumentId(dto.Id), Guid.Parse(dto.FileId), name, desc,
                            dto.TimeStamp)));

        public static Validation<Error, Event> ToEvent(this DocumentSentForApprovalEventDto dto) =>
            Validation<Error, Event>.Success(new DocumentSentForApprovalEvent(new DocumentId(dto.Id), dto.TimeStamp));

        public static Validation<Error, Event> ToEvent(this DocumentRejectedEventDto dto) =>
            RejectReason.Create(dto.Reason)
                .Map(reason => (Event) new DocumentRejectedEvent(new DocumentId(dto.Id), reason, dto.TimeStamp));
    }
}