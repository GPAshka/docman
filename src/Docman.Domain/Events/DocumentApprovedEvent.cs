using System;
using Docman.Domain.DocumentAggregate;

namespace Docman.Domain.Events
{
    public class DocumentApprovedEvent : Event
    {
        public DocumentApprovedEvent(Guid entityId, Comment comment) : base(entityId)
        {
            Comment = comment;
        }

        public DocumentApprovedEvent(Guid entityId, Comment comment, DateTime timeStamp) : base(entityId, timeStamp)
        {
            Comment = comment;
        }

        public Comment Comment { get; }
    }
}