using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Docman.API.Application.Commands;
using Docman.API.Application.Dto.Events;
using Docman.API.Application.Responses;
using Docman.API.Controllers;
using Docman.API.Extensions;
using Docman.Domain;
using Docman.Infrastructure.Repositories;
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

        private static DocumentRepository.DocumentExistsByNumber DocumentExistsByNumber =>
            number => Task.FromResult(false);

        [Fact]
        public async Task TestGetDocumentHistoryOkResult()
        {
            //Arrange
            var documentCreatedDto = new DocumentCreatedEventDto
                { Id = Guid.Empty, Number = "1234", TimeStamp = DateTime.UtcNow };
            var readEventsFunc = ValidReadEventsFunc(documentCreatedDto.ToEvent());
            
            _documentsController = new DocumentsController(readEventsFunc, SaveAndPublish, DocumentExistsByNumber);
            
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
            _documentsController = new DocumentsController(ValidReadEventsFunc(), SaveAndPublish, DocumentExistsByNumber);
            
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
            _documentsController = new DocumentsController(ReadEventsFuncWithError(error), SaveAndPublish, DocumentExistsByNumber);
            
            //Act
            var actionResult = await _documentsController.GetDocumentHistory(Guid.Empty);
            
            //Assert
            var badRequestResult = actionResult as BadRequestObjectResult; 
            Assert.NotNull(badRequestResult);
            Assert.NotNull(badRequestResult.Value);
        }
        
        [Fact]
        public async Task TestCreateDocumentCreatedResult()
        {
            //Arrange
            var command = new CreateDocumentCommand(Guid.Empty, "1234", "test");

            _documentsController = new DocumentsController(null, SaveAndPublish, DocumentExistsByNumber);
            
            //Act
            var result = await _documentsController.CreateDocument(command);
            
            //Assert
            var createdResult = result as CreatedResult; 
            Assert.NotNull(createdResult);
            Assert.NotNull(createdResult.Location);
        }
        
        [Fact]
        public async Task TestCreateDocumentInvalidCommandBadRequestResult()
        {
            //Arrange
            var command = new CreateDocumentCommand(Guid.Empty, string.Empty, "test");
            _documentsController = new DocumentsController(null, SaveAndPublish, DocumentExistsByNumber);
            
            //Act
            var result = await _documentsController.CreateDocument(command);
            
            //Assert
            var badRequestResult = result as BadRequestObjectResult; 
            Assert.NotNull(badRequestResult);
            Assert.NotNull(badRequestResult.Value);
        }
        
        [Fact]
        public async Task TestCreateDocumentDocumentExistsBadRequestResult()
        {
            //Arrange
            var command = new CreateDocumentCommand(Guid.Empty, "1234", "test");
            var documentExistsByNumber = new DocumentRepository.DocumentExistsByNumber(number => Task.FromResult(true));
            
            _documentsController = new DocumentsController(null, SaveAndPublish, documentExistsByNumber);
            
            //Act
            var result = await _documentsController.CreateDocument(command);
            
            //Assert
            var badRequestResult = result as BadRequestObjectResult; 
            Assert.NotNull(badRequestResult);
            Assert.NotNull(badRequestResult.Value);
        }
        
        [Fact]
        public async Task TestUpdateDocumentNoContentResult()
        {
            //Arrange
            var documentCreatedDto = new DocumentCreatedEventDto
                { Id = Guid.Empty, Number = "1234", TimeStamp = DateTime.UtcNow };
            var readEventsFunc = ValidReadEventsFunc(documentCreatedDto.ToEvent());
            
            var command = new UpdateDocumentCommand("1234", "test");
            _documentsController = new DocumentsController(readEventsFunc, SaveAndPublish, DocumentExistsByNumber);
            
            //Act
            var result = await _documentsController.UpdateDocument(Guid.Empty, command);
            
            //Assert
            var noContentResult = result as NoContentResult; 
            Assert.NotNull(noContentResult);
        }
        
        [Fact]
        public async Task TestUpdateDocumentInvalidCommandBadRequestResult()
        {
            //Arrange
            var documentCreatedDto = new DocumentCreatedEventDto
                { Id = Guid.Empty, Number = "1234", TimeStamp = DateTime.UtcNow };
            var readEventsFunc = ValidReadEventsFunc(documentCreatedDto.ToEvent());
            
            var command = new UpdateDocumentCommand(string.Empty, "test");
            _documentsController = new DocumentsController(readEventsFunc, SaveAndPublish, DocumentExistsByNumber);
            
            //Act
            var result = await _documentsController.UpdateDocument(Guid.Empty, command);
            
            //Assert
            var badRequestResult = result as BadRequestObjectResult; 
            Assert.NotNull(badRequestResult);
            Assert.NotNull(badRequestResult.Value);
        }
        
        [Fact]
        public async Task TestUpdateDocumentDocumentExistsBadRequestResult()
        {
            //Arrange
            var documentCreatedDto = new DocumentCreatedEventDto
                { Id = Guid.Empty, Number = "1234", TimeStamp = DateTime.UtcNow };
            var readEventsFunc = ValidReadEventsFunc(documentCreatedDto.ToEvent());
            
            var command = new UpdateDocumentCommand("1234", "test");
            var documentExistsByNumber = new DocumentRepository.DocumentExistsByNumber(number => Task.FromResult(true));
            
            _documentsController = new DocumentsController(readEventsFunc, SaveAndPublish, documentExistsByNumber);
            
            //Act
            var result = await _documentsController.UpdateDocument(Guid.Empty, command);
            
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
                { Id = Guid.Empty, Number = "1234", TimeStamp = DateTime.UtcNow };
            var fileAddedDto = new FileAddedEventDto
            {
                Id = Guid.Empty, FileId = Guid.Empty.ToString(), FileName = "test",
                TimeStamp = DateTime.UtcNow
            };
            var documentSentToApprovalDto = new DocumentSentForApprovalEventDto
                { Id = Guid.Empty, TimeStamp = DateTime.UtcNow };

            var readEventsFunc = ValidReadEventsFunc(documentCreatedDto.ToEvent(), fileAddedDto.ToEvent(),
                documentSentToApprovalDto.ToEvent());

            var command = new ApproveDocumentCommand("approve");
            
            _documentsController = new DocumentsController(readEventsFunc, SaveAndPublish, DocumentExistsByNumber);
            
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

            _documentsController = new DocumentsController(ReadEventsFuncWithError(error), SaveAndPublish, DocumentExistsByNumber);
            
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
                { Id = Guid.Empty, Number = "1234", TimeStamp = DateTime.UtcNow };
            var readEventsFunc = ValidReadEventsFunc(documentCreatedDto.ToEvent());

            _documentsController = new DocumentsController(readEventsFunc, SaveAndPublish, DocumentExistsByNumber);
            
            //Act
            var result = await _documentsController.ApproveDocument(Guid.Empty, command);
            
            //Assert
            var badRequestResult = result as BadRequestObjectResult; 
            Assert.NotNull(badRequestResult);
            Assert.NotNull(badRequestResult.Value);
        }
        
        [Fact]
        public async Task TestRejectDocumentNoContentResult()
        {
            //Arrange
            var documentCreatedDto = new DocumentCreatedEventDto
                { Id = Guid.Empty, Number = "1234", TimeStamp = DateTime.UtcNow };
            var fileAddedDto = new FileAddedEventDto
            {
                Id = Guid.Empty, FileId = Guid.Empty.ToString(), FileName = "test",
                TimeStamp = DateTime.UtcNow
            };
            var documentSentToApprovalDto = new DocumentSentForApprovalEventDto
                { Id = Guid.Empty, TimeStamp = DateTime.UtcNow };

            var readEventsFunc =
                ValidReadEventsFunc(documentCreatedDto.ToEvent(), fileAddedDto.ToEvent(), documentSentToApprovalDto.ToEvent());

            var command = new RejectDocumentCommand("Bad document");
            
            _documentsController = new DocumentsController(readEventsFunc, SaveAndPublish, DocumentExistsByNumber);
            
            //Act
            var result = await _documentsController.RejectDocument(Guid.Empty, command);
            
            //Assert
            var noContentResult = result as NoContentResult; 
            Assert.NotNull(noContentResult);
        }
        
        [Fact]
        public async Task TestRejectDocumentReadEventsErrorBadRequestResult()
        {
            //Arrange
            var command = new RejectDocumentCommand("Reject");
            const string error = "testError";

            _documentsController = new DocumentsController(ReadEventsFuncWithError(error), SaveAndPublish, DocumentExistsByNumber);
            
            //Act
            var result = await _documentsController.RejectDocument(Guid.Empty, command);
            
            //Assert
            var badRequestResult = result as BadRequestObjectResult; 
            Assert.NotNull(badRequestResult);
            Assert.NotNull(badRequestResult.Value);
        }
        
        [Fact]
        public async Task TestRejectDocumentInvalidCommandBadRequestResult()
        {
            //Arrange
            var command = new RejectDocumentCommand(string.Empty);
            var documentCreatedDto = new DocumentCreatedEventDto
                { Id = Guid.Empty, Number = "1234", TimeStamp = DateTime.UtcNow };
            var readEventsFunc = ValidReadEventsFunc(documentCreatedDto.ToEvent());

            _documentsController = new DocumentsController(readEventsFunc, SaveAndPublish, DocumentExistsByNumber);
            
            //Act
            var result = await _documentsController.RejectDocument(Guid.Empty, command);
            
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
                { Id = Guid.Empty, Number = "1234", TimeStamp = DateTime.UtcNow };
            var readEventsFunc = ValidReadEventsFunc(documentCreatedDto.ToEvent());
            
            _documentsController = new DocumentsController(readEventsFunc, SaveAndPublish, DocumentExistsByNumber);
            
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

            _documentsController = new DocumentsController(ReadEventsFuncWithError(error), SaveAndPublish, DocumentExistsByNumber);
            
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
                { Id = Guid.Empty, Number = "1234", TimeStamp = DateTime.UtcNow };
            var readEventsFunc = ValidReadEventsFunc(documentCreatedDto.ToEvent());

            _documentsController = new DocumentsController(readEventsFunc, SaveAndPublish, DocumentExistsByNumber);
            
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
                { Id = Guid.Empty, Number = "1234", TimeStamp = DateTime.UtcNow };
            var fileAddedDto = new FileAddedEventDto
            {
                Id = Guid.Empty, FileId = Guid.Empty.ToString(), FileName = "test",
                TimeStamp = DateTime.UtcNow
            };
            var readEventsFunc = ValidReadEventsFunc(documentCreatedDto.ToEvent(), fileAddedDto.ToEvent());

            _documentsController = new DocumentsController(readEventsFunc, SaveAndPublish, DocumentExistsByNumber);
            
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
                { Id = Guid.Empty, Number = "1234", TimeStamp = DateTime.UtcNow };
            var fileAddedDto = new FileAddedEventDto
            {
                Id = Guid.Empty, FileId = Guid.Empty.ToString(), FileName = "test",
                TimeStamp = DateTime.UtcNow
            };
            var readEventsFunc = ValidReadEventsFunc(documentCreatedDto.ToEvent(), fileAddedDto.ToEvent());
            
            _documentsController = new DocumentsController(readEventsFunc, SaveAndPublish, DocumentExistsByNumber);
            
            //Act
            var result = await _documentsController.SendDocumentForApproval(Guid.Empty);
            
            //Assert
            var noContentResult = result as NoContentResult; 
            Assert.NotNull(noContentResult);
        }
        
        [Fact]
        public async Task TestSendDocumentForApprovalNoFilesBadRequestResult()
        {
            //Arrange
            var documentCreatedDto = new DocumentCreatedEventDto
                { Id = Guid.Empty, Number = "1234", TimeStamp = DateTime.UtcNow };
            var readEventsFunc = ValidReadEventsFunc(documentCreatedDto.ToEvent());
            
            _documentsController = new DocumentsController(readEventsFunc, SaveAndPublish, DocumentExistsByNumber);
            
            //Act
            var result = await _documentsController.SendDocumentForApproval(Guid.Empty);
            
            //Assert
            var badRequestResult = result as BadRequestObjectResult; 
            Assert.NotNull(badRequestResult);
            Assert.NotNull(badRequestResult.Value);
        }
        
        [Fact]
        public async Task TestSendDocumentForApprovalInvalidIdBadRequestResult()
        {
            //Arrange
            var readEventsFunc = ValidReadEventsFunc();
            
            _documentsController = new DocumentsController(readEventsFunc, SaveAndPublish, DocumentExistsByNumber);
            
            //Act
            var result = await _documentsController.SendDocumentForApproval(Guid.Empty);
            
            //Assert
            var badRequestResult = result as BadRequestObjectResult; 
            Assert.NotNull(badRequestResult);
            Assert.NotNull(badRequestResult.Value);
        }
        
        [Fact]
        public async Task TestSendDocumentForApprovalReadEventsErrorBadRequestResult()
        {
            //Arrange
            const string error = "testError";
            _documentsController = new DocumentsController(ReadEventsFuncWithError(error), SaveAndPublish, DocumentExistsByNumber);
            
            //Act
            var result = await _documentsController.SendDocumentForApproval(Guid.Empty);
            
            //Assert
            var badRequestResult = result as BadRequestObjectResult; 
            Assert.NotNull(badRequestResult);
            Assert.NotNull(badRequestResult.Value);
        }
    }
}