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

        public Comment Comment { get; }
    }
}