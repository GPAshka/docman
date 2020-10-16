using System;
using System.Collections.Generic;
using LanguageExt;

namespace Docman.Domain.DocumentAggregate
{
    public class RejectedDocument : Document
    {
        public RejectReason Reason { get; }

        public RejectedDocument(Guid id, DocumentNumber number, Option<DocumentDescription> description,
            IEnumerable<File> files, RejectReason reason) : base(id, number, description, DocumentStatus.Rejected,
            files)
        {
            Reason = reason;
        }
    }
}