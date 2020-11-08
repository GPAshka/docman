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
            var number = DateTime.UtcNow.Ticks.ToString();
            const string description = "test document";
            
            // Act
            var documentUri = await _client.CreateDocumentAsync(number, description);
            var document = await _client.GetAsync<Document>(documentUri);
            
            // Assert
            Assert.NotNull(document);
            Assert.Equal(number, document.Number);
            Assert.Equal(description, document.Description);
            Assert.Equal(DocumentStatus.Draft.ToString(), document.Status);
        }

        [Fact]
        public async Task UpdateDocumentTest()
        {
            // Arrange
            const string description = "test document";
            var number = DateTime.UtcNow.Ticks.ToString();
            var updateDocumentCommand = new UpdateDocumentCommand($"{number}-update", $"{description}-update");

            // Act
            var documentUri = await _client.CreateDocumentAsync(number, description);
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
            var number = DateTime.UtcNow.Ticks.ToString();
            const string description = "test document";

            // Act
            var documentUri = await _client.CreateDocumentAsync(number, description);
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