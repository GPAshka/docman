using System;

namespace Docman.API.Dto.Events
{
    public class DocumentSentForApprovalEventDto
    {
        public string Id { get; set; }
        public DateTime TimeStamp { get; set; }
    }
}