using System;
using System.Collections.Generic;
using LanguageExt;

namespace Docman.Domain.DocumentAggregate
{
    public class ApprovedDocument : Document
    {
        public Comment Comment { get; }
        
        public ApprovedDocument(DocumentId id, UserId userId, DocumentNumber number,
            Option<DocumentDescription> description, IEnumerable<File> files, Comment comment) : base(id, userId,
            number, description, DocumentStatus.Approved, files)
        {
            Comment = comment;
        }
    }
}