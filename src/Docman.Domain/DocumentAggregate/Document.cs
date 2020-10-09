using System;
using System.Collections.Generic;
using Docman.Domain.Events;
using LanguageExt;

namespace Docman.Domain.DocumentAggregate
{
    public class Document : Record<Document>
    {
        public Guid Id { get; }
        public DocumentNumber Number { get; }
        public Option<DocumentDescription> Description { get; }
        public IEnumerable<File> Files { get; }
        public DocumentStatus Status { get; }

        public Document(Guid id, DocumentNumber number, Option<DocumentDescription> description,
            DocumentStatus status, IEnumerable<File> files)
        {
            Id = id;
            Number = number;
            Description = description;
            Status = status;
            Files = files;
        }
        
        public Document(Guid id, DocumentNumber number, Option<DocumentDescription> description,
            DocumentStatus status) : this(id, number, description, status, new List<File>())
        {
        }

        public Document WithFile(Guid id, FileName name, Option<FileDescription> description)
        {
            var file = new File(id, name, description);
            var newFiles = new List<File>(Files) { file };
            return new Document(Id, Number, Description, Status, newFiles);
        }
        
        public Validation<Error, (Document Document, DocumentApprovedEvent Event)> Approve(string comment)
        {
            if (Status != DocumentStatus.Draft)
                return new Error($"Document should have {DocumentStatus.Draft} status");

            return Comment.Create(comment)
                .Map(c => new DocumentApprovedEvent(Id, c))
                .Map(evt => (this.Apply(evt), evt));
        }

        public Validation<Error, (Document Document, FileAddedEvent Event)> AddFile(string fileName,
            string fileDescription)
        {
            if (Status != DocumentStatus.Draft)
                return new Error($"Document should have {DocumentStatus.Draft} status");

            return File.Create(Guid.NewGuid(), fileName, fileDescription)
                .Map(file => new FileAddedEvent(Id, file.Id, file.Name, file.Description))
                .Map(evt => (this.Apply(evt), evt));
        }
    }
}