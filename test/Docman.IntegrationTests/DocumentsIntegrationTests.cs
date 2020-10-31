using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Docman.API;
using Docman.API.Application.Commands;
using Docman.Domain.DocumentAggregate;
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
            
            var createDocumentCommand = new CreateDocumentCommand(number, description);
            var createDocumentJson = JsonConvert.SerializeObject(createDocumentCommand);
            var content = new StringContent(createDocumentJson, Encoding.UTF8, "application/json");
            
            // Act
            var createResponse = await _client.PostAsync("/documents", content);
            
            // Assert
            createResponse.EnsureSuccessStatusCode();
            Assert.NotNull(createResponse.Headers.Location);

            var getResponse = await _client.GetAsync(createResponse.Headers.Location);
            getResponse.EnsureSuccessStatusCode();

            var getDocumentJson = await getResponse.Content.ReadAsStringAsync();
            var document = JsonConvert.DeserializeObject<Document>(getDocumentJson);
            
            Assert.NotNull(document);
            Assert.Equal(number, document.Number);
            Assert.Equal(description, document.Description);
            Assert.Equal(DocumentStatus.Draft.ToString(), document.Status);
        }
    }
}