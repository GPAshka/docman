using MediatR;

namespace Docman.API.Application.Dto.Events
{
    public class DocumentUpdatedEventDto : EventDto, INotification
    {
        public string Number { get; set; }
        public string Description { get; set; }
    }
}