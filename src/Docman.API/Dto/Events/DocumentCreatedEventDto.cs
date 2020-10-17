using System;

namespace Docman.API.Dto.Events
{
    public class DocumentCreatedEventDto : EventDto
    {
        public string Number { get; set; }
        public string Description { get; set; }
    }
}