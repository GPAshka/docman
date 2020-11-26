using System;
using Docman.API.Application.Dto;
using Docman.API.Application.Dto.DocumentEvents.Events;
using Docman.API.Application.Dto.UserEvents;
using Docman.Domain;
using Docman.Domain.DocumentAggregate;
using Docman.Domain.DocumentAggregate.Events;
using Docman.Domain.UserAggregate;
using Docman.Domain.UserAggregate.Events;
using LanguageExt;
using UserId = Docman.Domain.DocumentAggregate.UserId;

namespace Docman.API.Application.Extensions
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
                DocumentUpdatedEvent documentUpdatedEvent => documentUpdatedEvent.ToDto(),
                UserCreatedEvent userCreatedEvent => userCreatedEvent.ToDto(),
                _ => throw new ArgumentOutOfRangeException(nameof(@event))    //TODO
            };

        private static DocumentCreatedEventDto ToDto(this DocumentCreatedEvent @event) =>
            new(@event.EntityId, @event.TimeStamp, @event.UserId.Value, @event.Number.Value,
                @event.Description.Match(Some: d => d.Value, None: string.Empty));

        private static DocumentApprovedEventDto ToDto(this DocumentApprovedEvent @event) =>
            new(@event.EntityId, @event.TimeStamp, @event.Comment.Value);

        private static FileAddedEventDto ToDto(this FileAddedEvent @event) =>
            new(@event.EntityId, @event.TimeStamp, @event.FileId.Value, @event.Name.Value,
                @event.Description.Match(d => d.Value, string.Empty));

        private static DocumentSentForApprovalEventDto ToDto(this DocumentSentForApprovalEvent @event) =>
            new(@event.EntityId, @event.TimeStamp);

        private static DocumentRejectedEventDto ToDto(this DocumentRejectedEvent @event) =>
            new(@event.EntityId, @event.TimeStamp, @event.Reason.Value);

        private static DocumentUpdatedEventDto ToDto(this DocumentUpdatedEvent @event) =>
            new(@event.EntityId, @event.TimeStamp, @event.Number.Value,
                @event.Description.Match(d => d.Value, string.Empty));

        private static UserCreatedEventDto ToDto(this UserCreatedEvent @event) => 
            new(@event.EntityId, @event.TimeStamp, @event.UserEmail.Value, @event.FirebaseId.Value);

        public static Validation<Error, Event> ToEvent(this DocumentCreatedEventDto dto) =>
            DocumentNumber.Create(dto.Number)
                .Bind(num => DocumentDescription.Create(dto.Description)
                    .Map(desc => (Event) new DocumentCreatedEvent(new DocumentId(dto.Id), new UserId(dto.UserId), num,
                        desc, dto.TimeStamp)));

        public static Validation<Error, Event> ToEvent(this DocumentApprovedEventDto dto) =>
            Comment.Create(dto.Comment)
                .Map(c => (Event) new DocumentApprovedEvent(new DocumentId(dto.Id), c, dto.TimeStamp));

        public static Validation<Error, Event> ToEvent(this FileAddedEventDto dto) =>
            FileName.Create(dto.FileName)
                .Bind(name => FileDescription.Create(dto.FileDescription)
                    .Map(desc =>
                        (Event) new FileAddedEvent(new DocumentId(dto.Id), new FileId(dto.FileId), name, desc,
                            dto.TimeStamp)));

        public static Validation<Error, Event> ToEvent(this DocumentSentForApprovalEventDto dto) =>
            Validation<Error, Event>.Success(new DocumentSentForApprovalEvent(new DocumentId(dto.Id), dto.TimeStamp));

        public static Validation<Error, Event> ToEvent(this DocumentRejectedEventDto dto) =>
            RejectReason.Create(dto.Reason)
                .Map(reason => (Event) new DocumentRejectedEvent(new DocumentId(dto.Id), reason, dto.TimeStamp));

        public static Validation<Error, Event> ToEvent(this DocumentUpdatedEventDto dto) =>
            DocumentNumber.Create(dto.Number)
                .Bind(num => DocumentDescription.Create(dto.Description)
                    .Map(desc => (Event) new DocumentUpdatedEvent(new DocumentId(dto.Id), num, desc, dto.TimeStamp)));

        public static Validation<Error, Event> ToEvent(this UserCreatedEventDto dto) =>
            Email.Create(dto.Email)
                .Map(email => (Event) new UserCreatedEvent(new Docman.Domain.UserAggregate.UserId(dto.Id),
                    dto.TimeStamp, email, new FirebaseId(dto.FirebaseId)));
    }
}