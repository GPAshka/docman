using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Docman.API.Commands;
using Docman.API.Controllers;
using Docman.API.Dto.Events;
using Docman.API.Extensions;
using Docman.API.Responses;
using Docman.Domain;
using Docman.Domain.Events;
using LanguageExt;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace Docman.UnitTests.Controllers
{
    public class DocumentControllerTests
    {
        private DocumentsController _documentsController;
        private static void SaveAndPublish(Event evt) { }

        private static Func<Guid, Task<Validation<Error, IEnumerable<Event>>>> ValidReadEventsFunc(
            params Validation<Error, Event>[] events) => id => Task.FromResult(events.Traverse(x => x));

        private static Func<Guid, Task<Validation<Error, IEnumerable<Event>>>> ReadEventsFuncWithError(string error) =>
            id => Task.FromResult(Validation<Error, IEnumerable<Event>>.Fail(new Seq<Error> { new Error(error) }));

        [Fact]
        public async Task TestGetDocumentHistoryOkResult()
        {
            //Arrange
            var documentCreatedDto = new DocumentCreatedEventDto
                { Id = Guid.Empty.ToString(), Number = "1234", TimeStamp = DateTime.UtcNow };
            var readEventsFunc = ValidReadEventsFunc(documentCreatedDto.ToEvent());
            
            _documentsController = new DocumentsController(readEventsFunc, SaveAndPublish);
            
            //Act
            var actionResult = await _documentsController.GetDocumentHistory(Guid.Empty);
            
            //Assert
            var okResult = actionResult as OkObjectResult;
            var documentHistory = okResult?.Value as IEnumerable<DocumentHistory>;
            
            Assert.NotNull(okResult);
            Assert.NotNull(documentHistory);
            Assert.Single(documentHistory);
        }
        
        [Fact]
        public async Task TestGetDocumentHistoryNotFoundResult()
        {
            //Arrange
            _documentsController = new DocumentsController(ValidReadEventsFunc(), SaveAndPublish);
            
            //Act
            var actionResult = await _documentsController.GetDocumentHistory(Guid.Empty);
            
            //Assert
            var notFoundResult = actionResult as NotFoundResult;
            Assert.NotNull(notFoundResult);
        }
        
        [Fact]
        public async Task TestGetDocumentHistoryBadRequestResult()
        {
            //Arrange
            const string error = "testError";
            _documentsController = new DocumentsController(ReadEventsFuncWithError(error), SaveAndPublish);
            
            //Act
            var actionResult = await _documentsController.GetDocumentHistory(Guid.Empty);
            
            //Assert
            var badRequestResult = actionResult as BadRequestObjectResult; 
            Assert.NotNull(badRequestResult);
            Assert.NotNull(badRequestResult.Value);
        }
        
        [Fact]
        public void TestCreateDocumentCreatedResult()
        {
            //Arrange
            var command = new CreateDocumentCommand(Guid.Empty, "1234", "test");

            _documentsController = new DocumentsController(null, SaveAndPublish);
            
            //Act
            var result = _documentsController.CreateDocument(command);
            
            //Assert
            var createdResult = result as CreatedResult; 
            Assert.NotNull(createdResult);
            Assert.NotNull(createdResult.Location);
        }
        
        [Fact]
        public void TestCreateDocumentBadRequestResult()
        {
            //Arrange
            var command = new CreateDocumentCommand(Guid.Empty, string.Empty, "test");
            _documentsController = new DocumentsController(null, SaveAndPublish);
            
            //Act
            var result = _documentsController.CreateDocument(command);
            
            //Assert
            var badRequestResult = result as BadRequestObjectResult; 
            Assert.NotNull(badRequestResult);
            Assert.NotNull(badRequestResult.Value);
        }
        
        [Fact]
        public async Task TestApproveDocumentNoContentResult()
        {
            //Arrange
            var documentCreatedDto = new DocumentCreatedEventDto
                { Id = Guid.Empty.ToString(), Number = "1234", TimeStamp = DateTime.UtcNow };
            var documentSentToApprovalDto = new DocumentSentForApprovalEventDto
                { Id = Guid.Empty.ToString(), TimeStamp = DateTime.UtcNow };

            var readEventsFunc =
                ValidReadEventsFunc(documentCreatedDto.ToEvent(), documentSentToApprovalDto.ToEvent());

            var command = new ApproveDocumentCommand("approve");
            
            _documentsController = new DocumentsController(readEventsFunc, SaveAndPublish);
            
            //Act
            var result = await _documentsController.ApproveDocument(Guid.Empty, command);
            
            //Assert
            var noContentResult = result as NoContentResult; 
            Assert.NotNull(noContentResult);
        }
        
        [Fact]
        public async Task TestApproveDocumentReadEventsErrorBadRequestResult()
        {
            //Arrange
            var command = new ApproveDocumentCommand("approve");
            const string error = "testError";

            _documentsController = new DocumentsController(ReadEventsFuncWithError(error), SaveAndPublish);
            
            //Act
            var result = await _documentsController.ApproveDocument(Guid.Empty, command);
            
            //Assert
            var badRequestResult = result as BadRequestObjectResult; 
            Assert.NotNull(badRequestResult);
            Assert.NotNull(badRequestResult.Value);
        }
        
        [Fact]
        public async Task TestApproveDocumentInvalidCommandBadRequestResult()
        {
            //Arrange
            var command = new ApproveDocumentCommand(null);
            var documentCreatedDto = new DocumentCreatedEventDto
                { Id = Guid.Empty.ToString(), Number = "1234", TimeStamp = DateTime.UtcNow };
            var readEventsFunc = ValidReadEventsFunc(documentCreatedDto.ToEvent());

            _documentsController = new DocumentsController(readEventsFunc, SaveAndPublish);
            
            //Act
            var result = await _documentsController.ApproveDocument(Guid.Empty, command);
            
            //Assert
            var badRequestResult = result as BadRequestObjectResult; 
            Assert.NotNull(badRequestResult);
            Assert.NotNull(badRequestResult.Value);
        }
        
        [Fact]
        public async Task TestAddFileCreatedResult()
        {
            //Arrange
            var documentId = Guid.NewGuid();
            var command = new AddFileCommand("test", "description");
            var documentCreatedDto = new DocumentCreatedEventDto
                { Id = Guid.Empty.ToString(), Number = "1234", TimeStamp = DateTime.UtcNow };
            var readEventsFunc = ValidReadEventsFunc(documentCreatedDto.ToEvent());
            
            _documentsController = new DocumentsController(readEventsFunc, SaveAndPublish);
            
            //Act
            var result = await _documentsController.AddFile(documentId, command);
            
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

            _documentsController = new DocumentsController(ReadEventsFuncWithError(error), SaveAndPublish);
            
            //Act
            var result = await _documentsController.AddFile(documentId, command);
            
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
                { Id = Guid.Empty.ToString(), Number = "1234", TimeStamp = DateTime.UtcNow };
            var readEventsFunc = ValidReadEventsFunc(documentCreatedDto.ToEvent());

            _documentsController = new DocumentsController(readEventsFunc, SaveAndPublish);
            
            //Act
            var result = await _documentsController.AddFile(documentId, command);
            
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
                { Id = Guid.Empty.ToString(), Number = "1234", TimeStamp = DateTime.UtcNow };
            var fileAddedDto = new FileAddedEventDto
            {
                DocumentId = Guid.Empty.ToString(), FileId = Guid.Empty.ToString(), FileName = "test",
                TimeStamp = DateTime.UtcNow
            };
            var readEventsFunc = ValidReadEventsFunc(documentCreatedDto.ToEvent(), fileAddedDto.ToEvent());

            _documentsController = new DocumentsController(readEventsFunc, SaveAndPublish);
            
            //Act
            var result = await _documentsController.AddFile(documentId, command);
            
            //Assert
            var badRequestResult = result as BadRequestObjectResult; 
            Assert.NotNull(badRequestResult);
            Assert.NotNull(badRequestResult.Value);
        }
        
        [Fact]
        public async Task TestSendDocumentForApprovalNoContentResult()
        {
            //Arrange
            var documentCreatedDto = new DocumentCreatedEventDto
                { Id = Guid.Empty.ToString(), Number = "1234", TimeStamp = DateTime.UtcNow };
            var readEventsFunc = ValidReadEventsFunc(documentCreatedDto.ToEvent());
            
            _documentsController = new DocumentsController(readEventsFunc, SaveAndPublish);
            
            //Act
            var result = await _documentsController.SendDocumentForApproval(Guid.Empty);
            
            //Assert
            var noContentResult = result as NoContentResult; 
            Assert.NotNull(noContentResult);
        }
        
        [Fact]
        public async Task TestSendDocumentForApprovalReadEventsErrorBadRequestResult()
        {
            //Arrange
            const string error = "testError";
            _documentsController = new DocumentsController(ReadEventsFuncWithError(error), SaveAndPublish);
            
            //Act
            var result = await _documentsController.SendDocumentForApproval(Guid.Empty);
            
            //Assert
            var badRequestResult = result as BadRequestObjectResult; 
            Assert.NotNull(badRequestResult);
            Assert.NotNull(badRequestResult.Value);
        }
    }
}