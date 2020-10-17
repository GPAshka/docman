using MediatR;

namespace Docman.API.Application.Dto.Events
{
    public class DocumentCreatedEventDto : EventDto, INotification
    {
        public string Number { get; set; }
        public string Description { get; set; }
    }
}