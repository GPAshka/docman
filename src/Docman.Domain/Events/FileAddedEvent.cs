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

        public static Validation<Error, FileAddedEvent> Create(Guid documentId, string fileName, string fileDescription)
            => FileName.Create(fileName)
                .Bind(name => FileDescription.Create(fileDescription)
                    .Map(desc =>
                        new FileAddedEvent(documentId, Guid.NewGuid(), name, desc, DateTime.UtcNow)));

    }
}