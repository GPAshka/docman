using System;
using System.Threading.Tasks;
using Docman.API;
using Docman.API.Application.Commands.Documents;
using Docman.Domain.DocumentAggregate;
using Docman.IntegrationTests.Extensions;
using Docman.IntegrationTests.Infrastructure;
using Xunit;
using Document = Docman.API.Application.Responses.Documents.Document;

namespace Docman.IntegrationTests
{
    public class DocumentsIntegrationTests : BaseIntegrationTests
    {
        public DocumentsIntegrationTests(CustomWebApplicationFactory<Startup> factory) : base(factory)
        {
        }

        [Fact]
        public async Task CreateDocumentTest()
        {
            // Arrange
            var createDocumentCommand = new CreateDocumentCommand(DateTime.UtcNow.Ticks.ToString(), "Test document");

            // Act
            var documentUri = await _client.CreateDocumentAsync(createDocumentCommand);
            var document = await _client.GetAsync<Document>(documentUri);
            
            // Assert
            Assert.NotNull(document);
            Assert.Equal(createDocumentCommand.Number, document.Number);
            Assert.Equal(createDocumentCommand.Description, document.Description);
            Assert.Equal(DocumentStatus.Draft.ToString(), document.Status);
        }

        [Fact]
        public async Task UpdateDocumentTest()
        {
            // Arrange
            var createDocumentCommand = new CreateDocumentCommand(DateTime.UtcNow.Ticks.ToString(), "Test document");
            var updateDocumentCommand = new UpdateDocumentCommand($"{createDocumentCommand.Number}-update",
                $"{createDocumentCommand.Description}-update");

            // Act
            var documentUri = await _client.CreateDocumentAsync(createDocumentCommand);
            await _client.UpdateDocumentAsync(documentUri, updateDocumentCommand);
            var document = await _client.GetAsync<Document>(documentUri);
            
            // Assert
            Assert.NotNull(document);
            Assert.Equal(updateDocumentCommand.Number, document.Number);
            Assert.Equal(updateDocumentCommand.Description, document.Description);
            Assert.Equal(DocumentStatus.Draft.ToString(), document.Status);
        }

        [Fact]
        public async Task SendDocumentForApprovalTest()
        {
            // Arrange
            var createDocumentCommand = new CreateDocumentCommand(DateTime.UtcNow.Ticks.ToString(), "Test document");
            var addFileCommand = new AddFileCommand("Test", "Test file");

            // Act
            var documentUri = await _client.CreateDocumentAsync(createDocumentCommand);
            await _client.AddFileAsync(documentUri, addFileCommand);
            await _client.SendDocumentForApprovalAsync(documentUri);
            
            var document = await _client.GetAsync<Document>(documentUri);
            
            // Assert
            Assert.NotNull(document);
            Assert.Equal(DocumentStatus.WaitingForApproval.ToString(), document.Status);
        }
        
        [Fact]
        public async Task ApproveDocumentTest()
        {
            // Arrange
            var createDocumentCommand = new CreateDocumentCommand(DateTime.UtcNow.Ticks.ToString(), "Test document");
            var addFileCommand = new AddFileCommand("Test", "Test file");
            var approveDocumentCommand = new ApproveDocumentCommand("Approved");

            // Act
            var documentUri = await _client.CreateDocumentAsync(createDocumentCommand);
            await _client.AddFileAsync(documentUri, addFileCommand);
            await _client.SendDocumentForApprovalAsync(documentUri);
            await _client.ApproveDocument(documentUri, approveDocumentCommand);

            var document = await _client.GetAsync<Document>(documentUri);
            
            // Assert
            Assert.NotNull(document);
            Assert.Equal(DocumentStatus.Approved.ToString(), document.Status);
            Assert.Equal(approveDocumentCommand.Comment, document.ApprovalComment);
        }
        
        [Fact]
        public async Task RejectDocumentTest()
        {
            // Arrange
            var createDocumentCommand = new CreateDocumentCommand(DateTime.UtcNow.Ticks.ToString(), "Test document");
            var addFileCommand = new AddFileCommand("Test", "Test file");
            var rejectDocumentCommand = new RejectDocumentCommand("Rejected");

            // Act
            var documentUri = await _client.CreateDocumentAsync(createDocumentCommand);
            await _client.AddFileAsync(documentUri, addFileCommand);
            await _client.SendDocumentForApprovalAsync(documentUri);
            await _client.RejectDocument(documentUri, rejectDocumentCommand);

            var document = await _client.GetAsync<Document>(documentUri);
            
            // Assert
            Assert.NotNull(document);
            Assert.Equal(DocumentStatus.Rejected.ToString(), document.Status);
            Assert.Equal(rejectDocumentCommand.Reason, document.RejectReason);
        }
    }
}