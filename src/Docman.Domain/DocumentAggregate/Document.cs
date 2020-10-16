using System;
using System.Collections.Generic;
using System.Linq;
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

        public Validation<Error, Document> AddFile(Guid id, FileName name, Option<FileDescription> description)
        {
            if (Status != DocumentStatus.Draft)
                return new Error($"Document should have {DocumentStatus.Draft} status");
            
            if (Files.Any(f => f.Name.Equals(name)))
                return new Error($"Document already has file with name '{name.Value}'");
            
            var file = new File(id, name, description);
            var newFiles = new List<File>(Files) { file };
            return new Document(Id, Number, Description, Status, newFiles);
        }

        public Validation<Error, Document> WaitingForApproval()
        {
            if (Status != DocumentStatus.Draft)
                return new Error($"Document should have {DocumentStatus.Draft} status");
            
            if (!Files.Any())
                return new Error("Document should have at least one file");

            return WithStatus(DocumentStatus.WaitingForApproval);
        }

        public Validation<Error, Document> Approve(Comment comment)
        {
            if (Status != DocumentStatus.WaitingForApproval)
                return new Error($"Document should have {DocumentStatus.WaitingForApproval} status");
            
            return new ApprovedDocument(Id, Number, Description, Files, comment);   
        }
        
        public Validation<Error, Document> Reject(RejectReason reason)
        {
            if (Status != DocumentStatus.WaitingForApproval)
                return new Error($"Document should have {DocumentStatus.WaitingForApproval} status");
            
            return new RejectedDocument(Id, Number, Description, Files, reason);   
        }

        private Document WithStatus(DocumentStatus status) =>
            new Document(Id, Number, Description, status, Files);
    }
}