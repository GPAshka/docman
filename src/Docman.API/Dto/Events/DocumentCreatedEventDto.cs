using System;

namespace Docman.API.Dto.Events
{
    public class DocumentCreatedEventDto
    {
        public string Id { get; set; }
        public string Number { get; set; }
        public string Description { get; set; }
        public DateTime TimeStamp { get; set; }
    }
}