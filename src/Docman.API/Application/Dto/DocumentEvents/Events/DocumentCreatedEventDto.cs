using System;
using MediatR;

namespace Docman.API.Application.Dto.DocumentEvents.Events
{
    public record DocumentCreatedEventDto : EventDto, INotification
    {
        public Guid UserId { get; }
        public string Number { get; }
        public string Description { get; }

        public DocumentCreatedEventDto(Guid id, DateTime timeStamp, Guid userId, string number, string description) :
            base(id, timeStamp)
        {
            UserId = userId;
            Number = number;
            Description = description;
        }
    }
}