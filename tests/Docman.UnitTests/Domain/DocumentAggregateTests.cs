using System;
using System.Linq;
using Docman.Domain;
using Docman.Domain.DocumentAggregate;
using Docman.Domain.Errors;
using LanguageExt;
using Xunit;

namespace Docman.UnitTests.Domain
{
    public class DocumentAggregateTests
    {
        [Fact]
        public void CreateDocumentTest()
        {
            var document = CreateDocument("1234", "Test Document");
            
            document.Match(
                Succ: doc =>
                {
                    Assert.Equal("1234", doc.Number.Value);
                    Assert.Equal(DocumentStatus.Draft, doc.Status);

                    doc.Description.Match(
                        Some: desc => Assert.Equal("Test Document", desc.Value),
                        None: () => Assert.True(false, "Should never get here"));
                },
                Fail: errors => Assert.True(false, "Should never get here"));
        }
        
        [Fact]
        public void CreateDocumentEmptyDescriptionTest()
        {
            var document = CreateDocument("123", string.Empty);
            
            document.Match(
                Succ: doc =>
                {
                    Assert.Equal("123", doc.Number.Value);
                    Assert.Equal(DocumentStatus.Draft, doc.Status);

                    doc.Description.Match(
                        Some: _ => Assert.True(false, "Should never get here"),
                        None: () => Assert.True(true));
                },
                Fail: errors => Assert.True(false, "Should never get here"));
        }
        
        [Fact]
        public void CreateDocumentInvalidNumberErrorTest()
        {
            var document = CreateDocument(string.Empty, "Test Document");
            
            document.Match(
                Succ: _ => Assert.True(false, "Should never get here"),
                Fail: errors =>
                {
                    Assert.True(errors.Count == 1);
                    Assert.True(errors.Head.Equals(new EmptyValueError("Document number")));
                });
        }
        
        [Fact]
        public void CreateDocumentInvalidDescriptionErrorTest()
        {
            var document = CreateDocument("123",
                new string(Enumerable.Repeat('t', DocumentDescription.MaxLength + 1).ToArray()));
            
            document.Match(
                Succ: _ => Assert.True(false, "Should never get here"),
                Fail: errors =>
                {
                    Assert.True(errors.Count == 1);
                    Assert.True(errors.Head.Equals(new LongValueError("Document description",
                        DocumentDescription.MaxLength)));
                });
        }
        
        [Fact]
        public void AddFileSuccessTest()
        {
            var document = CreateDocument()
                .Bind(doc => AddFile(doc, "123", "Test File"));
            
            document.Match(
                Succ: doc =>
                {
                    Assert.NotNull(doc.Files);
                    Assert.NotEmpty(doc.Files);
                    Assert.Equal(1, doc.Files.Count());
                    Assert.Equal("123", doc.Files.First().Name.Value);
                    Assert.Equal(DocumentStatus.Draft, doc.Status);

                    doc.Files.First().Description.Match(
                        Some: desc => Assert.Equal("Test File", desc.Value),
                        None: () => Assert.True(false, "Should never get here"));
                },
                Fail: errors => Assert.True(false, "Should never get here"));
        }
        
        [Fact]
        public void AddFileInvalidFileNameErrorTest()
        {
            var document = CreateDocument()
                .Bind(doc => AddFile(doc, string.Empty, "TestFile"));
            
            document.Match(
                Succ: _ => Assert.True(false, "Should never get here"),
                Fail: errors =>
                {
                    Assert.True(errors.Count == 1);
                    Assert.True(errors.Head.Equals(new EmptyValueError("File name")));
                });
        }
        
        [Fact]
        public void AddFileInvalidFileDescriptionErrorTest()
        {
            var document = CreateDocument()
                .Bind(doc => AddFile(doc, "123", new string(Enumerable.Repeat('t', 300).ToArray())));

            Assert.True(document.IsFail);
            document.Match(
                Succ: _ => Assert.True(false, "Should never get here"),
                Fail: errors =>
                {
                    Assert.True(errors.Count == 1);
                    Assert.True(errors.Head.Equals(new LongValueError("File description", FileDescription.MaxLength)));
                });
        }
        
        [Fact]
        public void AddFileEmptyFileDescriptionTest()
        {
            var document = CreateDocument()
                .Bind(doc => AddFile(doc, "123", string.Empty));

            document.Match(
                Succ: doc =>
                {
                    Assert.NotNull(doc.Files);
                    Assert.NotEmpty(doc.Files);
                    Assert.Equal(1, doc.Files.Count());
                    Assert.Equal("123", doc.Files.First().Name.Value);
                    Assert.True(doc.Files.First().Description.IsNone);
                    Assert.Equal(DocumentStatus.Draft, doc.Status);
                },
                Fail: errors => Assert.True(false, "Should never get here"));
        }
        
        [Fact]
        public void AddFileInvalidDocumentStatusErrorTest()
        {
            var document = CreateDocument()
                .Bind(doc => AddFile(doc, "123"))
                .Bind(doc => doc.SendForApproval())
                .Bind(doc => AddFile(doc, "1233"));

            Assert.True(document.IsFail);
            document.Match(
                Succ: _ => Assert.True(false, "Should never get here"),
                Fail: errors =>
                {
                    Assert.True(errors.Count == 1);
                    Assert.True(errors.Head.Equals(new InvalidStatusError(DocumentStatus.Draft,
                        DocumentStatus.WaitingForApproval)));
                });
        }

        [Fact]
        public void AddFileExistsErrorTest()
        {
            var document = CreateDocument()
                .Bind(doc => AddFile(doc, "123"))
                .Bind(doc => AddFile(doc, "123"));
            
            document.Match(
                Succ: _ => Assert.True(false, "Should never get here"),
                Fail: errors =>
                {
                    Assert.True(errors.Count == 1);
                    Assert.True(errors.Head.Equals(new FileExistsError("123")));
                });
        }
        
        [Fact]
        public void SendForApprovalTest()
        {
            var document = CreateDocument()
                .Bind(doc => AddFile(doc, "123"))
                .Bind(doc => doc.SendForApproval());

            document.Match(
                Succ: doc => Assert.Equal(DocumentStatus.WaitingForApproval, doc.Status),
                Fail: errors => Assert.True(false, "Should never get here"));
        }
        
        [Fact]
        public void SendForApprovalNoFilesErrorTest()
        {
            var document = CreateDocument()
                .Bind(doc => doc.SendForApproval());

            document.Match(
                Succ: _ => Assert.True(false, "Should never get here"),
                Fail: errors =>
                {
                    Assert.True(errors.Count == 1);
                    Assert.True(errors.Head.Equals(new NoFilesError()));
                });
        }
        
        [Fact]
        public void SendForApprovalInvalidDocumentStatusErrorTest()
        {
            var document = CreateDocument()
                .Bind(doc => AddFile(doc))
                .Bind(doc => doc.SendForApproval())
                .Bind(doc => doc.SendForApproval());

            document.Match(
                Succ: _ => Assert.True(false, "Should never get here"),
                Fail: errors =>
                {
                    Assert.True(errors.Count == 1);
                    Assert.True(errors.Head.Equals(new InvalidStatusError(DocumentStatus.Draft,
                        DocumentStatus.WaitingForApproval)));
                });
        }
        
        [Fact]
        public void ApproveTest()
        {
            var document = CreateDocument()
                .Bind(doc => AddFile(doc, "123"))
                .Bind(doc => doc.SendForApproval())
                .Bind(doc => Comment.Create("Approved")
                    .Bind(doc.Approve));

            document.Match(
                Succ: doc => Assert.Equal(DocumentStatus.Approved, doc.Status),
                Fail: errors => Assert.True(false, "Should never get here"));
        }
        
        [Fact]
        public void ApproveInvalidDocumentStatusErrorTest()
        {
            var document = CreateDocument()
                .Bind(doc => AddFile(doc, "123"))
                .Bind(doc => Comment.Create("Approved")
                    .Bind(doc.Approve));

            document.Match(
                Succ: _ => Assert.True(false, "Should never get here"),
                Fail: errors =>
                {
                    Assert.True(errors.Count == 1);
                    Assert.True(errors.Head.Equals(new InvalidStatusError(DocumentStatus.WaitingForApproval,
                        DocumentStatus.Draft)));
                });
        }
        
        [Fact]
        public void ApproveEmptyCommentErrorTest()
        {
            var document = CreateDocument()
                .Bind(doc => AddFile(doc, "123"))
                .Bind(doc => Comment.Create(string.Empty)
                    .Bind(doc.Approve));

            document.Match(
                Succ: _ => Assert.True(false, "Should never get here"),
                Fail: errors =>
                {
                    Assert.True(errors.Count == 1);
                    Assert.True(errors.Head.Equals(new EmptyValueError("Comment")));
                });
        }
        
        [Fact]
        public void RejectTest()
        {
            var document = CreateDocument()
                .Bind(doc => AddFile(doc, "123"))
                .Bind(doc => doc.SendForApproval())
                .Bind(doc => RejectReason.Create("Rejected")
                    .Bind(doc.Reject));

            document.Match(
                Succ: doc => Assert.Equal(DocumentStatus.Rejected, doc.Status),
                Fail: errors => Assert.True(false, "Should never get here"));
        }
        
        [Fact]
        public void RejectInvalidDocumentStatusErrorTest()
        {
            var document = CreateDocument()
                .Bind(doc => AddFile(doc, "123"))
                .Bind(doc => RejectReason.Create("Rejected")
                    .Bind(doc.Reject));

            document.Match(
                Succ: _ => Assert.True(false, "Should never get here"),
                Fail: errors =>
                {
                    Assert.True(errors.Count == 1);
                    Assert.True(errors.Head.Equals(new InvalidStatusError(DocumentStatus.WaitingForApproval,
                        DocumentStatus.Draft)));
                });
        }
        
        [Fact]
        public void RejectEmptyReasonErrorTest()
        {
            var document = CreateDocument()
                .Bind(doc => AddFile(doc, "123"))
                .Bind(doc => RejectReason.Create(string.Empty)
                    .Bind(doc.Reject));

            document.Match(
                Succ: _ => Assert.True(false, "Should never get here"),
                Fail: errors =>
                {
                    Assert.True(errors.Count == 1);
                    Assert.True(errors.Head.Equals(new EmptyValueError("Reject reason")));
                });
        }
        
        private static Validation<Error, Document> CreateDocument(string number = "1234", string description = "Test")
        {
            return DocumentNumber.Create(number)
                .Bind(num => DocumentDescription.Create(description)
                    .Map(desc => new Document(new DocumentId(Guid.Empty), new UserId(Guid.Empty), num, desc)));
        }

        private static Validation<Error, Document> AddFile(Validation<Error, Document> document,
            string fileName = "File123", string fileDescription = "Test file")
        {
            return document
                .Bind(doc => FileName.Create(fileName)
                    .Bind(name => FileDescription.Create(fileDescription)
                        .Bind(fileDesc => doc.AddFile(new FileId(Guid.Empty), name, fileDesc))));
        }
    }
}