using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Docman.API.Application.Commands;
using Docman.API.Application.Dto.Events;
using Docman.API.Application.Extensions;
using Docman.API.Application.Responses;
using Docman.API.Controllers;
using Docman.Infrastructure.Dto;
using Docman.Infrastructure.Repositories;
using LanguageExt;
using Microsoft.AspNetCore.Mvc;
using Xunit;
using static LanguageExt.Prelude;

namespace Docman.UnitTests.Controllers
{
    public class DocumentsControllerTests
    {
        private DocumentsController _documentsController;

        private static DocumentRepository.DocumentExistsByNumber DocumentExistsByNumber =>
            number => Task.FromResult(false);

        private static DocumentRepository.GetDocumentById GetDocumentById =>
            documentId => Task.FromResult(Some(new DocumentDatabaseDto { Id = documentId }));

        [Fact]
        public async Task TestGetOkResult()
        {
            //Arrange
            var documentId = Guid.NewGuid();
            _documentsController =
                new DocumentsController(TestHelper.ValidReadEventsFunc(), TestHelper.SaveAndPublish, DocumentExistsByNumber, GetDocumentById);

            // Act
            var actionResult = await _documentsController.Get(documentId);
            
            // Assert
            var okResult = actionResult as OkObjectResult;
            var document = okResult?.Value as Document;
            
            Assert.NotNull(okResult);
            Assert.NotNull(document);
            Assert.Equal(documentId, document.Id);
        }
        
        [Fact]
        public async Task TestGetNotFoundResult()
        {
            //Arrange
            var documentId = Guid.Empty;
            var getDocumentById =
                new DocumentRepository.GetDocumentById(id => Task.FromResult(Option<DocumentDatabaseDto>.None));
            _documentsController = 
                new DocumentsController(TestHelper.ValidReadEventsFunc(), TestHelper.SaveAndPublish, DocumentExistsByNumber, getDocumentById);

            // Act
            var actionResult = await _documentsController.Get(documentId);
            
            // Assert
            var notFoundResult = actionResult as NotFoundResult;

            Assert.NotNull(notFoundResult);
        }
        
        [Fact]
        public async Task TestGetDocumentHistoryOkResult()
        {
            //Arrange
            var documentCreatedDto =
                new DocumentCreatedEventDto(Guid.Empty, DateTime.UtcNow, Guid.Empty, "1234", string.Empty);
            var readEventsFunc = TestHelper.ValidReadEventsFunc(documentCreatedDto.ToEvent());

            _documentsController =
                new DocumentsController(readEventsFunc, TestHelper.SaveAndPublish, DocumentExistsByNumber, GetDocumentById);
            
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
            _documentsController = new DocumentsController(TestHelper.ValidReadEventsFunc(), TestHelper.SaveAndPublish,
                DocumentExistsByNumber, GetDocumentById);
            
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
            _documentsController = new DocumentsController(TestHelper.ReadEventsFuncWithError(error), TestHelper.SaveAndPublish,
                DocumentExistsByNumber, GetDocumentById);
            
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
            var command = new CreateDocumentCommand( Guid.Empty, "1234", "test");

            _documentsController =
                new DocumentsController(TestHelper.ValidReadEventsFunc(), TestHelper.SaveAndPublish, DocumentExistsByNumber, GetDocumentById);
            
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
            _documentsController =
                new DocumentsController(TestHelper.ValidReadEventsFunc(), TestHelper.SaveAndPublish, DocumentExistsByNumber, GetDocumentById);
            
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

            _documentsController =
                new DocumentsController(TestHelper.ValidReadEventsFunc(), TestHelper.SaveAndPublish, documentExistsByNumber, GetDocumentById);
            
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
            var documentCreatedDto =
                new DocumentCreatedEventDto(Guid.Empty, DateTime.UtcNow, Guid.Empty, "1234", string.Empty);
            var readEventsFunc = TestHelper.ValidReadEventsFunc(documentCreatedDto.ToEvent());
            
            var command = new UpdateDocumentCommand("1234", "test");
            _documentsController =
                new DocumentsController(readEventsFunc, TestHelper.SaveAndPublish, DocumentExistsByNumber, GetDocumentById);
            
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
            var documentCreatedDto =
                new DocumentCreatedEventDto(Guid.Empty, DateTime.UtcNow, Guid.Empty, "1234", string.Empty);
            var readEventsFunc = TestHelper.ValidReadEventsFunc(documentCreatedDto.ToEvent());
            
            var command = new UpdateDocumentCommand(string.Empty, "test");
            _documentsController =
                new DocumentsController(readEventsFunc, TestHelper.SaveAndPublish, DocumentExistsByNumber, GetDocumentById);
            
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
            var documentCreatedDto =
                new DocumentCreatedEventDto(Guid.Empty, DateTime.UtcNow, Guid.Empty, "1234", string.Empty);
            var readEventsFunc = TestHelper.ValidReadEventsFunc(documentCreatedDto.ToEvent());
            
            var command = new UpdateDocumentCommand("1234", "test");
            var documentExistsByNumber = new DocumentRepository.DocumentExistsByNumber(number => Task.FromResult(true));

            _documentsController =
                new DocumentsController(readEventsFunc, TestHelper.SaveAndPublish, documentExistsByNumber, GetDocumentById);
            
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
            var documentCreatedDto =
                new DocumentCreatedEventDto(Guid.Empty, DateTime.UtcNow, Guid.Empty, "1234", string.Empty);
            var fileAddedDto = new FileAddedEventDto(Guid.Empty, DateTime.UtcNow, Guid.Empty, "test", string.Empty);
            var documentSentToApprovalDto = new DocumentSentForApprovalEventDto(Guid.Empty, DateTime.UtcNow);

            var readEventsFunc = TestHelper.ValidReadEventsFunc(documentCreatedDto.ToEvent(), fileAddedDto.ToEvent(),
                documentSentToApprovalDto.ToEvent());

            var command = new ApproveDocumentCommand("approve");

            _documentsController =
                new DocumentsController(readEventsFunc, TestHelper.SaveAndPublish, DocumentExistsByNumber, GetDocumentById);
            
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

            _documentsController = new DocumentsController(TestHelper.ReadEventsFuncWithError(error), TestHelper.SaveAndPublish,
                DocumentExistsByNumber, GetDocumentById);
            
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
            var documentCreatedDto =
                new DocumentCreatedEventDto(Guid.Empty, DateTime.UtcNow, Guid.Empty, "1234", string.Empty);
            var readEventsFunc = TestHelper.ValidReadEventsFunc(documentCreatedDto.ToEvent());

            _documentsController =
                new DocumentsController(readEventsFunc, TestHelper.SaveAndPublish, DocumentExistsByNumber, GetDocumentById);
            
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
            var documentCreatedDto =
                new DocumentCreatedEventDto(Guid.Empty, DateTime.UtcNow, Guid.Empty, "1234", string.Empty);
            var fileAddedDto = new FileAddedEventDto(Guid.Empty, DateTime.UtcNow, Guid.Empty, "test", string.Empty);
            var documentSentToApprovalDto = new DocumentSentForApprovalEventDto(Guid.Empty, DateTime.UtcNow);

            var readEventsFunc = TestHelper.ValidReadEventsFunc(documentCreatedDto.ToEvent(), fileAddedDto.ToEvent(), documentSentToApprovalDto.ToEvent());

            var command = new RejectDocumentCommand("Bad document");

            _documentsController =
                new DocumentsController(readEventsFunc, TestHelper.SaveAndPublish, DocumentExistsByNumber, GetDocumentById);
            
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

            _documentsController = new DocumentsController(TestHelper.ReadEventsFuncWithError(error), TestHelper.SaveAndPublish,
                DocumentExistsByNumber, GetDocumentById);
            
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
            var documentCreatedDto =
                new DocumentCreatedEventDto(Guid.Empty, DateTime.UtcNow, Guid.Empty, "1234", string.Empty);
            var readEventsFunc = TestHelper.ValidReadEventsFunc(documentCreatedDto.ToEvent());

            _documentsController =
                new DocumentsController(readEventsFunc, TestHelper.SaveAndPublish, DocumentExistsByNumber, GetDocumentById);
            
            //Act
            var result = await _documentsController.RejectDocument(Guid.Empty, command);
            
            //Assert
            var badRequestResult = result as BadRequestObjectResult; 
            Assert.NotNull(badRequestResult);
            Assert.NotNull(badRequestResult.Value);
        }

        [Fact]
        public async Task TestSendDocumentForApprovalNoContentResult()
        {
            //Arrange
            var documentCreatedDto =
                new DocumentCreatedEventDto(Guid.Empty, DateTime.UtcNow, Guid.Empty, "1234", string.Empty);
            var fileAddedDto = new FileAddedEventDto(Guid.Empty, DateTime.UtcNow, Guid.Empty, "test", string.Empty);
            var readEventsFunc = TestHelper.ValidReadEventsFunc(documentCreatedDto.ToEvent(), fileAddedDto.ToEvent());

            _documentsController =
                new DocumentsController(readEventsFunc, TestHelper.SaveAndPublish, DocumentExistsByNumber, GetDocumentById);
            
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
            var documentCreatedDto =
                new DocumentCreatedEventDto(Guid.Empty, DateTime.UtcNow, Guid.Empty, "1234", string.Empty);
            var readEventsFunc = TestHelper.ValidReadEventsFunc(documentCreatedDto.ToEvent());

            _documentsController =
                new DocumentsController(readEventsFunc, TestHelper.SaveAndPublish, DocumentExistsByNumber, GetDocumentById);
            
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
            var readEventsFunc = TestHelper.ValidReadEventsFunc();

            _documentsController =
                new DocumentsController(readEventsFunc, TestHelper.SaveAndPublish, DocumentExistsByNumber, GetDocumentById);
            
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
            _documentsController = new DocumentsController(TestHelper.ReadEventsFuncWithError(error), TestHelper.SaveAndPublish,
                DocumentExistsByNumber, GetDocumentById);
            
            //Act
            var result = await _documentsController.SendDocumentForApproval(Guid.Empty);
            
            //Assert
            var badRequestResult = result as BadRequestObjectResult; 
            Assert.NotNull(badRequestResult);
            Assert.NotNull(badRequestResult.Value);
        }
    }
}