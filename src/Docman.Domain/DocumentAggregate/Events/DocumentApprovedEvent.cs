using System;
using LanguageExt;

namespace Docman.Domain.DocumentAggregate.Events
{
    public class DocumentApprovedEvent : Event
    {
        public Comment Comment { get; }

        public DocumentApprovedEvent(DocumentId entityId, Comment comment, DateTime timeStamp) : base(entityId.Value,
            timeStamp)
        {
            Comment = comment;
        }

        public static Validation<Error, DocumentApprovedEvent> Create(Guid documentId, string comment) =>
            Comment.Create(comment)
                .Map(c => new DocumentApprovedEvent(new DocumentId(documentId), c, DateTime.UtcNow));
    }
}