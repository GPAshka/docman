using System;
using LanguageExt;

namespace Docman.Domain.DocumentAggregate
{
    public class DocumentId : NewType<DocumentId, Guid>
    {
        public DocumentId(Guid value) : base(value)
        {
        }
    }
}