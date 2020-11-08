using System;
using System.Net.Http;
using System.Threading.Tasks;
using Docman.API;
using Docman.API.Application.Commands;
using Docman.API.Application.Responses;
using Docman.IntegrationTests.Extensions;
using Docman.IntegrationTests.Infrastructure;
using Xunit;

namespace Docman.IntegrationTests
{
    public class DocumentFilesIntegrationTests : IClassFixture<CustomWebApplicationFactory<Startup>>
    {
        private readonly HttpClient _client;

        public DocumentFilesIntegrationTests(CustomWebApplicationFactory<Startup> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task AddFileTest()
        {
            // Arrange
            var number = DateTime.UtcNow.Ticks.ToString();
            const string documentDescription = "test document";
            var addFileCommand = new AddFileCommand("Test", "Test file");

            // Act
            var documentUri = await _client.CreateDocumentAsync(number, documentDescription);
            var fileUri = await _client.AddFileAsync(documentUri, addFileCommand);
            var file = await _client.GetAsync<File>(fileUri);

            // Assert
            Assert.NotNull(file);
            Assert.Equal(addFileCommand.FileName, file.Name);
            Assert.Equal(addFileCommand.FileDescription, file.Description);
        }
    }
}