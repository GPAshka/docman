using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Docman.API.Application.Commands;
using Docman.API.Application.Dto.Events;
using Docman.API.Application.Responses;
using Docman.API.Controllers;
using Docman.API.Extensions;
using Docman.Infrastructure.Dto;
using Docman.Infrastructure.Repositories;
using LanguageExt;
using Microsoft.AspNetCore.Mvc;
using Xunit;
using static LanguageExt.Prelude;

namespace Docman.UnitTests.Controllers
{
    public class DocumentFilesControllerTests
    {
        private DocumentFilesController _documentFilesController;

        private static DocumentRepository.GetFile GetFile =>
            (documentId, fileId) => Task.FromResult(Some(new FileDatabaseDto { Id = fileId, DocumentId = documentId }));

        private static DocumentRepository.GetFiles GetFiles => documentId =>
        {
            IEnumerable<FileDatabaseDto> files = new List<FileDatabaseDto>
                { new FileDatabaseDto { DocumentId = documentId } };
            return Task.FromResult(files);
        };

        [Fact]
        public async Task TestGetFileOkResult()
        {
            //Arrange
            var fileId = Guid.NewGuid();
            _documentFilesController =
                new DocumentFilesController(Helper.ValidReadEventsFunc(), Helper.SaveAndPublish, GetFile, GetFiles);

            // Act
            var actionResult = await _documentFilesController.GetFileAsync(Guid.Empty, fileId);
            
            // Assert
            var okResult = actionResult as OkObjectResult;
            var file = okResult?.Value as File;
            
            Assert.NotNull(okResult);
            Assert.NotNull(file);
            Assert.Equal(fileId, file.Id);
        }
        
        [Fact]
        public async Task TestGetFileNotFoundResult()
        {
            //Arrange
            var fileId = Guid.NewGuid();
            var getFileById =
                new DocumentRepository.GetFile((documentId, id) => Task.FromResult(Option<FileDatabaseDto>.None));
            _documentFilesController =
                new DocumentFilesController(Helper.ValidReadEventsFunc(), Helper.SaveAndPublish, getFileById, GetFiles);

            // Act
            var actionResult = await _documentFilesController.GetFileAsync(Guid.Empty, fileId);
            
            // Assert
            var notFoundResult = actionResult as NotFoundResult;
            Assert.NotNull(notFoundResult);
        }

        [Fact]
        public async Task TestGetFilesOkResult()
        {
            //Arrange
            _documentFilesController =
                new DocumentFilesController(Helper.ValidReadEventsFunc(), Helper.SaveAndPublish, GetFile, GetFiles);

            // Act
            var actionResult = await _documentFilesController.GetFilesAsync(Guid.Empty);
            
            // Assert
            var okResult = actionResult as OkObjectResult;
            var files = okResult?.Value as IEnumerable<File>;
            
            Assert.NotNull(okResult);
            Assert.NotNull(files);
            Assert.Single(files);
        }
        
        [Fact]
        public async Task TestGetFilesEmptyOkResult()
        {
            //Arrange
            var getFiles =
                new DocumentRepository.GetFiles(documentId => Task.FromResult(Enumerable.Empty<FileDatabaseDto>()));
            _documentFilesController =
                new DocumentFilesController(Helper.ValidReadEventsFunc(), Helper.SaveAndPublish, GetFile, getFiles);

            // Act
            var actionResult = await _documentFilesController.GetFilesAsync(Guid.Empty);
            
            // Assert
            var okResult = actionResult as OkObjectResult;
            var files = okResult?.Value as IEnumerable<File>;
            
            Assert.NotNull(okResult);
            Assert.NotNull(files);
            Assert.Empty(files);
        }

        [Fact]
        public async Task TestAddFileCreatedResult()
        {
            //Arrange
            var documentId = Guid.NewGuid();
            var command = new AddFileCommand("test", "description");
            var documentCreatedDto = new DocumentCreatedEventDto
                { Id = Guid.Empty, Number = "1234", TimeStamp = DateTime.UtcNow };
            var readEventsFunc = Helper.ValidReadEventsFunc(documentCreatedDto.ToEvent());

            _documentFilesController =
                new DocumentFilesController(readEventsFunc, Helper.SaveAndPublish, GetFile, GetFiles);
            
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

            _documentFilesController = new DocumentFilesController(Helper.ReadEventsFuncWithError(error),
                Helper.SaveAndPublish, GetFile, GetFiles);
            
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

            _documentFilesController =
                new DocumentFilesController(readEventsFunc, Helper.SaveAndPublish, GetFile, GetFiles);
            
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
                Id = Guid.Empty, FileId = Guid.Empty, FileName = "test", TimeStamp = DateTime.UtcNow
            };
            var readEventsFunc =
                Helper.ValidReadEventsFunc(documentCreatedDto.ToEvent(), fileAddedDto.ToEvent());

            _documentFilesController =
                new DocumentFilesController(readEventsFunc, Helper.SaveAndPublish, GetFile, GetFiles);
            
            //Act
            var result = await _documentFilesController.AddFile(documentId, command);
            
            //Assert
            var badRequestResult = result as BadRequestObjectResult; 
            Assert.NotNull(badRequestResult);
            Assert.NotNull(badRequestResult.Value);
        }
    }
}