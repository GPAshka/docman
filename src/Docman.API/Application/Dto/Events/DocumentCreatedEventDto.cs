using System;
using MediatR;

namespace Docman.API.Application.Dto.Events
{
    public class DocumentCreatedEventDto : EventDto, INotification
    {
        public Guid UserId { get; set; }
        public string Number { get; set; }
        public string Description { get; set; }
    }
}