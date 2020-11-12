using System;
using LanguageExt;

namespace Docman.Domain.DocumentAggregate.Events
{
    public class FileAddedEvent : Event
    {
        public FileId FileId { get; }
        public FileName Name { get; }
        public Option<FileDescription> Description { get; }

        public FileAddedEvent(DocumentId entityId, FileId fileId, FileName name, Option<FileDescription> description,
            DateTime timeStamp) : base(entityId.Value, timeStamp)
        {
            FileId = fileId;
            Name = name;
            Description = description;
        }

        public static Validation<Error, FileAddedEvent> Create(Guid documentId, Guid fileId, string fileName,
            string fileDescription) =>
            FileName.Create(fileName)
                .Bind(name => FileDescription.Create(fileDescription)
                    .Map(desc =>
                        new FileAddedEvent(new DocumentId(documentId), new FileId(fileId), name, desc,
                            DateTime.UtcNow)));

    }
}