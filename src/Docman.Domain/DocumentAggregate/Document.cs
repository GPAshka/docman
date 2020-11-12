using System.Collections.Generic;
using System.Linq;
using Docman.Domain.DocumentAggregate.Errors;
using LanguageExt;

namespace Docman.Domain.DocumentAggregate
{
    public class Document : Record<Document>
    {
        public DocumentId Id { get; }
        public UserId UserId { get; }
        public DocumentNumber Number { get; }
        public Option<DocumentDescription> Description { get; }
        public IEnumerable<File> Files { get; }
        public DocumentStatus Status { get; }

        protected Document(DocumentId id, UserId userId, DocumentNumber number, Option<DocumentDescription> description,
            DocumentStatus status, IEnumerable<File> files)
        {
            Id = id;
            UserId = userId;
            Number = number;
            Description = description;
            Status = status;
            Files = files;
        }
        
        public Document(DocumentId id, UserId userId, DocumentNumber number, Option<DocumentDescription> description,
            DocumentStatus status) : this(id, userId, number, description, status, new List<File>())
        {
        }

        public Validation<Error, Document> AddFile(FileId id, FileName name, Option<FileDescription> description)
        {
            if (Status != DocumentStatus.Draft)
                return new InvalidStatusError(DocumentStatus.Draft, Status);

            if (Files.Any(f => f.Name.Equals(name)))
                return new FileExistsError(name.Value);

            var file = new File(id, name, description);
            var newFiles = new List<File>(Files) { file };
            return new Document(Id, UserId, Number, Description, Status, newFiles);
        }

        public Validation<Error, Document> WaitingForApproval()
        {
            if (Status != DocumentStatus.Draft)
                return new InvalidStatusError(DocumentStatus.Draft, Status);
            
            if (!Files.Any())
                return new NoFilesError();

            return WithStatus(DocumentStatus.WaitingForApproval);
        }

        public Validation<Error, Document> Approve(Comment comment)
        {
            if (Status != DocumentStatus.WaitingForApproval)
                return new InvalidStatusError(DocumentStatus.WaitingForApproval, Status);

            return new ApprovedDocument(Id, UserId, Number, Description, Files, comment);   
        }
        
        public Validation<Error, Document> Reject(RejectReason reason)
        {
            if (Status != DocumentStatus.WaitingForApproval)
                return new InvalidStatusError(DocumentStatus.WaitingForApproval, Status);
            
            return new RejectedDocument(Id, UserId, Number, Description, Files, reason);   
        }

        public Validation<Error, Document> Update(DocumentNumber number, Option<DocumentDescription> description)
        {
            if (Status != DocumentStatus.Draft)
                return new InvalidStatusError(DocumentStatus.Draft, Status);

            return new Document(Id, UserId, number, description, Status, Files);
        }

        private Document WithStatus(DocumentStatus status) =>
            new Document(Id, UserId, Number, Description, status, Files);
    }
}