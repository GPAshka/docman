using System;
using LanguageExt;

namespace Docman.Domain.DocumentAggregate
{
    public class ApprovedDocument : Document
    {
        public ApprovedDocument(Guid id, DocumentNumber number, Option<DocumentDescription> description,
            Comment comment) : base(id, number, description, DocumentStatus.Approved)
        {
            Comment = comment;
        }

        public Comment Comment { get; }
    }
}