using MediatR;

namespace Docman.API.Application.Dto.Events
{
    public class DocumentApprovedEventDto : EventDto, INotification
    {
        public string Comment { get; set; }
    }
}