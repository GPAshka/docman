namespace Docman.API.Application.Dto.Events
{
    public class DocumentUpdatedEventDto : EventDto
    {
        public string Number { get; set; }
        public string Description { get; set; }
    }
}