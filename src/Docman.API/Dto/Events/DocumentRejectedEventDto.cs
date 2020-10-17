using System;

namespace Docman.API.Dto.Events
{
    public class DocumentRejectedEventDto : EventDto
    {
        public string Reason { get; set; }
    }
}