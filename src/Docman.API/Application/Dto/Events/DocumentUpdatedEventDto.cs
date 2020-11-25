using System;
using MediatR;

namespace Docman.API.Application.Dto.Events
{
    public record DocumentUpdatedEventDto : EventDto, INotification
    {
        public string Number { get; }
        public string Description { get; }

        public DocumentUpdatedEventDto(Guid id, DateTime timeStamp, string number, string description) : base(id,
            timeStamp)
        {
            Number = number;
            Description = description;
        }
    }
}