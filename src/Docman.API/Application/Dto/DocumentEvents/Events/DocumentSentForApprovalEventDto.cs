using System;
using MediatR;

namespace Docman.API.Application.Dto.DocumentEvents.Events
{
    public record DocumentSentForApprovalEventDto : EventDto, INotification
    {
        public DocumentSentForApprovalEventDto(Guid id, DateTime timeStamp) : base(id, timeStamp)
        {
        }
    }
}