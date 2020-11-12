using MediatR;

namespace Docman.API.Application.Dto.Events
{
    public class DocumentRejectedEventDto : EventDto, INotification
    {
        public string Reason { get; set; }
    }
}