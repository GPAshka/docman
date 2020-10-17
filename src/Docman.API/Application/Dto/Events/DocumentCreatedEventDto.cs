namespace Docman.API.Application.Dto.Events
{
    public class DocumentCreatedEventDto : EventDto
    {
        public string Number { get; set; }
        public string Description { get; set; }
    }
}