using System;
using System.Threading.Tasks;
using Docman.API.Application.Commands;
using Docman.API.Application.Dto.Events;
using Docman.API.Controllers;
using Docman.API.Extensions;
using Docman.Domain;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace Docman.UnitTests.Controllers
{
    public class DocumentFilesControllerTests
    {
        private DocumentFilesController _documentFilesController;
        private static void SaveAndPublish(Event evt) { }
        
        [Fact]
        public async Task TestAddFileCreatedResult()
        {
            //Arrange
            var documentId = Guid.NewGuid();
            var command = new AddFileCommand("test", "description");
            var documentCreatedDto = new DocumentCreatedEventDto
                { Id = Guid.Empty, Number = "1234", TimeStamp = DateTime.UtcNow };
            var readEventsFunc = Helper.ValidReadEventsFunc(documentCreatedDto.ToEvent());

            _documentFilesController = new DocumentFilesController(readEventsFunc, SaveAndPublish);
            
            //Act
            var result = await _documentFilesController.AddFile(documentId, command);
            
            //Assert
            var createdResult = result as CreatedResult; 
            Assert.NotNull(createdResult);
            Assert.NotNull(createdResult.Location);
        }
        
        [Fact]
        public async Task TestAddFileReadEventsErrorBadRequestResult()
        {
            //Arrange
            const string error = "testError";
            var documentId = Guid.NewGuid();
            var command = new AddFileCommand("test", "description");

            _documentFilesController =
                new DocumentFilesController(Helper.ReadEventsFuncWithError(error), SaveAndPublish);
            
            //Act
            var result = await _documentFilesController.AddFile(documentId, command);
            
            //Assert
            var badRequestResult = result as BadRequestObjectResult; 
            Assert.NotNull(badRequestResult);
            Assert.NotNull(badRequestResult.Value);
        }
        
        [Fact]
        public async Task TestAddFileInvalidCommandBadRequestResult()
        {
            //Arrange
            var documentId = Guid.NewGuid();
            var command = new AddFileCommand(null, "test");
            var documentCreatedDto = new DocumentCreatedEventDto
                { Id = Guid.Empty, Number = "1234", TimeStamp = DateTime.UtcNow };
            var readEventsFunc = Helper.ValidReadEventsFunc(documentCreatedDto.ToEvent());

            _documentFilesController = new DocumentFilesController(readEventsFunc, SaveAndPublish);
            
            //Act
            var result = await _documentFilesController.AddFile(documentId, command);
            
            //Assert
            var badRequestResult = result as BadRequestObjectResult; 
            Assert.NotNull(badRequestResult);
            Assert.NotNull(badRequestResult.Value);
        }
        
        [Fact]
        public async Task TestAddFileFIleNameExistsBadRequestResult()
        {
            //Arrange
            var documentId = Guid.NewGuid();
            var command = new AddFileCommand("test", "test");
            
            var documentCreatedDto = new DocumentCreatedEventDto
                { Id = Guid.Empty, Number = "1234", TimeStamp = DateTime.UtcNow };
            var fileAddedDto = new FileAddedEventDto
            {
                Id = Guid.Empty, FileId = Guid.Empty.ToString(), FileName = "test",
                TimeStamp = DateTime.UtcNow
            };
            var readEventsFunc =
                Helper.ValidReadEventsFunc(documentCreatedDto.ToEvent(), fileAddedDto.ToEvent());

            _documentFilesController = new DocumentFilesController(readEventsFunc, SaveAndPublish);
            
            //Act
            var result = await _documentFilesController.AddFile(documentId, command);
            
            //Assert
            var badRequestResult = result as BadRequestObjectResult; 
            Assert.NotNull(badRequestResult);
            Assert.NotNull(badRequestResult.Value);
        }
    }
}