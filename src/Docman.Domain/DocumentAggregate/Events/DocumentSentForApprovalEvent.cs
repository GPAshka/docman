using System;

namespace Docman.Domain.DocumentAggregate.Events
{
    public class DocumentSentForApprovalEvent : Event
    {
        public DocumentSentForApprovalEvent(DocumentId entityId) : this(entityId, DateTime.UtcNow)
        {
        }

        public DocumentSentForApprovalEvent(DocumentId entityId, DateTime timeStamp) : base(entityId.Value, timeStamp)
        {
        }
    }
}