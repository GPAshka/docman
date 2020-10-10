using System;
using System.Collections.Generic;
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

        protected Document(Guid id, DocumentNumber number, Option<DocumentDescription> description,
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

        public Document WithStatus(DocumentStatus status) =>
            new Document(Id, Number, Description, status, Files);

        public ApprovedDocument Approve(Comment comment) =>
            new ApprovedDocument(Id, Number, Description, Files, comment);
    }
}