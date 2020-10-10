using System;
using Docman.Domain.DocumentAggregate;
using LanguageExt;

namespace Docman.Domain.Events
{
    public class DocumentApprovedEvent : Event
    {
        public Comment Comment { get; }
        
        public DocumentApprovedEvent(Guid entityId, Comment comment) : base(entityId)
        {
            Comment = comment;
        }

        public DocumentApprovedEvent(Guid entityId, Comment comment, DateTime timeStamp) : base(entityId, timeStamp)
        {
            Comment = comment;
        }

        public static Validation<Error, DocumentApprovedEvent> Create(Guid documentId, string comment) =>
            Comment.Create(comment)
                .Map(c => new DocumentApprovedEvent(documentId, c));
    }
}