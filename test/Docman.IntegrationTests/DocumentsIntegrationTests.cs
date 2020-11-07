using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Docman.API;
using Docman.API.Application.Commands;
using Docman.Domain.DocumentAggregate;
using Docman.IntegrationTests.Infrastructure;
using Newtonsoft.Json;
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
            var documentUri = await CreateDocumentAsync(number, description);
            var document = await GetDocumentAsync(documentUri);
            
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
            var content = GetStringContent(updateDocumentCommand);
            
            // Act
            var documentUri = await CreateDocumentAsync(number, description);
            var documentUpdateResult = await _client.PutAsync(documentUri, content);
            var document = await GetDocumentAsync(documentUri);
            
            // Assert
            documentUpdateResult.EnsureSuccessStatusCode();
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
            var documentUri = await CreateDocumentAsync(number, description);
            var sendForApprovalResult = await _client.PutAsync(
                new Uri(Path.Combine(documentUri.OriginalString, "send-for-approval"), UriKind.Relative),
                new StringContent(string.Empty));
            var document = await GetDocumentAsync(documentUri);
            
            // Assert
            sendForApprovalResult.EnsureSuccessStatusCode();
            Assert.NotNull(document);
            Assert.Equal(DocumentStatus.WaitingForApproval.ToString(), document.Status);
        }

        private async Task<Uri> CreateDocumentAsync(string number, string description)
        {
            var createDocumentCommand = new CreateDocumentCommand(number, description);
            var content = GetStringContent(createDocumentCommand);

            var createResponse = await _client.PostAsync("/documents", content);
            
            createResponse.EnsureSuccessStatusCode();
            Assert.NotNull(createResponse.Headers.Location);

            return createResponse.Headers.Location;
        }

        private async Task<Document> GetDocumentAsync(Uri documentUri)
        {
            var getResponse = await _client.GetAsync(documentUri);
            getResponse.EnsureSuccessStatusCode();

            var getDocumentJson = await getResponse.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<Document>(getDocumentJson);
        }

        private static StringContent GetStringContent(object command)
        {
            var json = JsonConvert.SerializeObject(command);
            return new StringContent(json, Encoding.UTF8, "application/json");
        }
    }
}