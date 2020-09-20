using System;

namespace Docman.Infrastructure.EventDto
{
    public class DocumentApprovedEventDto
    {
        public string Id { get; set; }
        public string Comment { get; set; }
        public DateTime TimeStamp { get; set; }
    }
}