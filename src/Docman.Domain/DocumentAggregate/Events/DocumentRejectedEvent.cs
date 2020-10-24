using System;
using LanguageExt;

namespace Docman.Domain.DocumentAggregate.Events
{
    public class DocumentRejectedEvent : Event
    {
        public RejectReason Reason { get; }

        public DocumentRejectedEvent(DocumentId entityId, RejectReason reason, DateTime timeStamp) : base(
            entityId.Value, timeStamp)
        {
            Reason = reason;
        }

        public static Validation<Error, DocumentRejectedEvent> Create(DocumentId documentId, string reason) =>
            RejectReason.Create(reason)
                    .Map(r => new DocumentRejectedEvent(documentId, r, DateTime.UtcNow));
    }
}