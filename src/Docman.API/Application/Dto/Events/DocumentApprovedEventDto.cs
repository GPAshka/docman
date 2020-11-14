using System;
using MediatR;

namespace Docman.API.Application.Dto.Events
{
    public class DocumentApprovedEventDto : EventDto, INotification
    {
        public string Comment { get; }

        public DocumentApprovedEventDto(Guid id, DateTime timeStamp, string comment) : base(id, timeStamp)
        {
            Comment = comment;
        }
    }
}