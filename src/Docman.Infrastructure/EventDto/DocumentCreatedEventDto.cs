using System;

namespace Docman.Infrastructure.EventDto
{
    public class DocumentCreatedEventDto
    {
        public string Id { get; set; }
        public string Number { get; set; }
        public string Description { get; set; }
        public DateTime TimeStamp { get; set; }
    }
}