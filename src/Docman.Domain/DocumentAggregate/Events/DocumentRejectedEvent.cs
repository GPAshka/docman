using System;
using LanguageExt;

namespace Docman.Domain.DocumentAggregate.Events
{
    public class DocumentRejectedEvent : Event
    {
        public RejectReason Reason { get; }

        public DocumentRejectedEvent(Guid entityId, RejectReason reason) : base(entityId)
        {
            Reason = reason;
        }

        public DocumentRejectedEvent(Guid entityId, RejectReason reason, DateTime timeStamp) : base(entityId, timeStamp)
        {
            Reason = reason;
        }

        public static Validation<Error, DocumentRejectedEvent> Create(Guid documentId, string reason) =>
            RejectReason.Create(reason)
                .Map(r => new DocumentRejectedEvent(documentId, r));
    }
}