using System;
using System.Collections.Generic;
using LanguageExt;

namespace Docman.Domain.DocumentAggregate
{
    public class ApprovedDocument : Document
    {
        public ApprovedDocument(Guid id, DocumentNumber number, Option<DocumentDescription> description,
            IEnumerable<File> files, Comment comment) : base(id, number, description, DocumentStatus.Approved, files)
        {
            Comment = comment;
        }

        public Comment Comment { get; }
    }
}