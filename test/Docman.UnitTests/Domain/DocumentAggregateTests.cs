using System;
using System.Linq;
using Docman.Domain;
using Docman.Domain.DocumentAggregate;
using Docman.Domain.DocumentAggregate.Errors;
using LanguageExt;
using Xunit;

namespace Docman.UnitTests.Domain
{
    public class DocumentAggregateTests
    {
        [Fact]
        public void AddFileSuccessTest()
        {
            var document = CreateDocument()
                .Bind(doc => AddFile(doc));
            
            Assert.True(document.IsSuccess);
        }
        
        [Fact]
        public void AddFileInvalidFileNameErrorTest()
        {
            var document = CreateDocument()
                .Bind(doc => AddFile(doc, string.Empty, "TestFile"));

            Assert.True(document.IsFail);
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

            Assert.True(document.IsSuccess);
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

            Assert.True(document.IsSuccess);
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