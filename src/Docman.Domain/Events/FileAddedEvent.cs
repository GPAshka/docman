using System;
using Docman.Domain.DocumentAggregate;
using LanguageExt;

namespace Docman.Domain.Events
{
    public class FileAddedEvent : Event
    {
        public Guid FileId { get; }
        public FileName Name { get; }
        public Option<FileDescription> Description { get; }

        public FileAddedEvent(Guid entityId, Guid fileId, FileName name, Option<FileDescription> description,
            DateTime timeStamp) : base(entityId, timeStamp)
        {
            FileId = fileId;
            Name = name;
            Description = description;
        }
    }
}