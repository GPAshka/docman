using System;
using MediatR;

namespace Docman.API.Application.Dto.Events
{
    public class DocumentRejectedEventDto : EventDto, INotification
    {
        public string Reason { get; }

        public DocumentRejectedEventDto(Guid id, DateTime timeStamp, string reason) : base(id, timeStamp)
        {
            Reason = reason;
        }
    }
}