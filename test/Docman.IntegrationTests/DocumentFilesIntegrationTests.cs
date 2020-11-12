using System;
using System.Linq;
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
            var createDocumentCommand =
                new CreateDocumentCommand(Guid.NewGuid(), DateTime.UtcNow.Ticks.ToString(), "Test document");
            var addFileCommand = new AddFileCommand("Test", "Test file");

            // Act
            var documentUri = await _client.CreateDocumentAsync(createDocumentCommand);
            var fileUri = await _client.AddFileAsync(documentUri, addFileCommand);
            var file = await _client.GetAsync<File>(fileUri);
            var files = await _client.GetFiles(documentUri);

            // Assert
            Assert.NotNull(file);
            Assert.Equal(addFileCommand.FileName, file.Name);
            Assert.Equal(addFileCommand.FileDescription, file.Description);
            
            Assert.NotNull(files);
            Assert.Single(files);
            Assert.Equal(file.Id, files.First().Id);
        }
    }
}