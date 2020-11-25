using System;
using MediatR;

namespace Docman.API.Application.Dto.Events
{
    public record DocumentSentForApprovalEventDto : EventDto, INotification
    {
        public DocumentSentForApprovalEventDto(Guid id, DateTime timeStamp) : base(id, timeStamp)
        {
        }
    }
}