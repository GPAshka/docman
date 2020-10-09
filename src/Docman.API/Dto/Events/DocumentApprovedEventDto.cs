using System;

namespace Docman.API.Dto.Events
{
    public class DocumentApprovedEventDto
    {
        public string Id { get; set; }
        public string Comment { get; set; }
        public DateTime TimeStamp { get; set; }
    }
}