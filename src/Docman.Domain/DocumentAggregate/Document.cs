using System.Collections.Generic;
using System.Linq;
using Docman.Domain.Errors;
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

        public Document(DocumentId id, UserId userId, DocumentNumber number, Option<DocumentDescription> description) :
            this(id, userId, number, description, DocumentStatus.Draft, new List<File>())
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
            return WithFiles(newFiles);
        }

        public Validation<Error, Document> SendForApproval()
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

            return WithNumber(number).WithDescription(description);
        }

        private Document WithStatus(DocumentStatus status) =>
            new Document(Id, UserId, Number, Description, status, Files);

        private Document WithFiles(List<File> files) => new Document(Id, UserId, Number, Description, Status, files);

        private Document WithNumber(DocumentNumber number) =>
            new Document(Id, UserId, number, Description, Status, Files);

        private Document WithDescription(Option<DocumentDescription> description) =>
            new Document(Id, UserId, Number, description, Status, Files);
    }
}