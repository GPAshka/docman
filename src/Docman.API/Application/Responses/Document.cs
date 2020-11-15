using System;
using LanguageExt;

namespace Docman.API.Application.Responses
{
    public class Document : Record<Document>
    {
        public Document(Guid id, Guid userId, string number, string description, string status, string? approvalComment,
            string? rejectReason)
        {
            Id = id;
            UserId = userId;
            Number = number;
            Description = description;
            Status = status;
            ApprovalComment = approvalComment;
            RejectReason = rejectReason;
        }

        public Guid Id { get; }
        public Guid UserId { get; }
        public string Number { get; }
        public string Description { get; }
        public string Status { get; }
        public string? ApprovalComment { get; }
        public string? RejectReason { get; }
    }
}