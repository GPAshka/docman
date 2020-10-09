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

        public FileAddedEvent(Guid entityId, Guid fileId, FileName name, Option<FileDescription> description) :
            base(entityId)
        {
            FileId = fileId;
            Name = name;
            Description = description;
        }

        public FileAddedEvent(Guid entityId, DateTime timeStamp, Guid fileId, FileName name,
            Option<FileDescription> description) : base(entityId, timeStamp)
        {
            FileId = fileId;
            Name = name;
            Description = description;
        }
    }
}