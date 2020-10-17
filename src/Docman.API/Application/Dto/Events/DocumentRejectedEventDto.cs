namespace Docman.API.Application.Dto.Events
{
    public class DocumentRejectedEventDto : EventDto
    {
        public string Reason { get; set; }
    }
}