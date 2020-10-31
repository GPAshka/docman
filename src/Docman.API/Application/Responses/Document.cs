using System;

namespace Docman.API.Application.Responses
{
    public class Document
    {
        public Guid Id { get; set; }
        public string Number { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public string? ApprovalComment { get; set; }
        public string? RejectReason { get; set; }
    }
}