using System;

namespace Docman.API.Dto.Events
{
    public class DocumentRejectedEventDto
    {
        public string Id { get; set; }
        public string Reason { get; set; }
        public DateTime TimeStamp { get; set; }
    }
}