using System;

namespace Docman.API.Dto.Events
{
    public class DocumentApprovedEventDto : EventDto
    {
        public string Comment { get; set; }
    }
}