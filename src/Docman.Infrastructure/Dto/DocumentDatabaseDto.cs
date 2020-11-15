using System;
using LanguageExt;

namespace Docman.Infrastructure.Dto
{
    public class DocumentDatabaseDto : Record<DocumentDatabaseDto>
    {
        public Guid Id { get; }
        public Guid UserId { get; }
        public string Number { get; }
        public string Description { get; }
        public string Status { get; }
        public string? ApprovalComment { get; }
        public string? RejectReason { get; }
        public DateTime DateCreated { get; }

        public DocumentDatabaseDto(Guid id, Guid userId, string number, string description, string status,
            string? approvalComment, string? rejectReason, DateTime dateCreated)
        {
            Id = id;
            UserId = userId;
            Number = number;
            Description = description;
            Status = status;
            ApprovalComment = approvalComment;
            RejectReason = rejectReason;
            DateCreated = dateCreated;
        }
    }
}