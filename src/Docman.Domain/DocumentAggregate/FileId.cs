using System;
using LanguageExt;

namespace Docman.Domain.DocumentAggregate
{
    public class FileId : NewType<FileId, Guid>
    {
        public FileId(Guid value) : base(value)
        {
        }
    }
}