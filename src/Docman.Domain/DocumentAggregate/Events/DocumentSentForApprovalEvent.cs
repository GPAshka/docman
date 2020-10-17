using System;

namespace Docman.Domain.DocumentAggregate.Events
{
    public class DocumentSentForApprovalEvent : Event
    {
        public DocumentSentForApprovalEvent(Guid entityId) : base(entityId)
        {
        }

        public DocumentSentForApprovalEvent(Guid entityId, DateTime timeStamp) : base(entityId, timeStamp)
        {
        }
    }
}