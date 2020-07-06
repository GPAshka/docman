using System;
using Docman.API.CommandHandlers;
using Docman.API.Commands;
using Docman.Domain.DocumentAggregate;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Docman.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DocumentsController : ControllerBase
    {
        private readonly ILogger<DocumentsController> _logger;

        public DocumentsController(ILogger<DocumentsController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        [Route("{documentId:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetDocument(Guid documentId)
        {
            return Ok();
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public IActionResult CreateDocument([FromBody] CreateDocumentCommand command)
        {
            if (command.DocumentId == Guid.Empty)
                command.DocumentId = Guid.NewGuid();
            
            //handle command
            var (evt, newState) = DocumentCommandHandlers.Create(command);
            
            // TODO save and publish event
            
            return CreatedAtAction(nameof(GetDocument), new {documentId = command.DocumentId}, newState);
        }

        [HttpPost]
        [Route("approve")]
        public IActionResult ApproveDocument([FromBody] ApproveDocumentCommand command)
        {
            //TODO get document
            //var document = GetDocument(command.DocumentId);
            var document = new Document(Guid.NewGuid(), new DocumentNumber("test"));
            
            //perform state transition
            var (evt, newState) = document.Approve(command);
            
            // TODO save and publish event

            return NoContent();
        }
    }
}