using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Docman.API;
using Docman.API.Application.Commands;
using Docman.Domain.DocumentAggregate;
using Docman.IntegrationTests.Extensions;
using Docman.IntegrationTests.Infrastructure;
using Xunit;
using Document = Docman.API.Application.Responses.Document;

namespace Docman.IntegrationTests
{
    public class DocumentsIntegrationTests : IClassFixture<CustomWebApplicationFactory<Startup>>
    {
        private readonly HttpClient _client;

        public DocumentsIntegrationTests(CustomWebApplicationFactory<Startup> factory)
        {
            _client = factory.CreateClient();
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

            // Act
            var documentUri = await _client.CreateDocumentAsync(createDocumentCommand);
            var sendForApprovalResponse = await _client.PutAsync(
                new Uri(Path.Combine(documentUri.OriginalString, "send-for-approval"), UriKind.Relative),
                new StringContent(string.Empty));
            var document = await _client.GetAsync<Document>(documentUri);
            
            // Assert
            sendForApprovalResponse.EnsureSuccessStatusCode();
            Assert.NotNull(document);
            Assert.Equal(DocumentStatus.WaitingForApproval.ToString(), document.Status);
        }
    }
}