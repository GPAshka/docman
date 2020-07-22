using System;
using Docman.API.CommandHandlers;
using Docman.API.Commands;
using Docman.API.Requests;
using Docman.Domain;
using Docman.Domain.DocumentAggregate;
using Docman.Infrastructure.EventStore;
using LanguageExt;
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
        public IActionResult CreateDocument([FromBody] CreateDocumentRequest request)
        {
            if (request.DocumentId == Guid.Empty)
                request.DocumentId = Guid.NewGuid();

            // TODO validate request
            
            var command = new CreateDocumentCommand(request.DocumentId, new DocumentNumber(request.DocumentNumber));
            
            //handle command
            var (evt, newState) = DocumentCommandHandlers.Create(command);
            
            // TODO save and publish event

            var eventsRepository = new EventsRepository(new Uri("tcp://admin:changeit@127.0.0.1:1113"));
            eventsRepository.AddEvent(evt);
            
            return CreatedAtAction(nameof(GetDocument), new {documentId = command.DocumentId}, newState);
        }

        [HttpPost]
        [Route("approve")]
        public IActionResult ApproveDocument([FromBody] ApproveDocumentCommand command)
        {
            //TODO validate request
            //TODO get document
            
            var eventsRepository = new EventsRepository(new Uri("tcp://admin:changeit@127.0.0.1:1113"));
            return eventsRepository
                .ReadEvents(command.DocumentId)
                .Map(DocumentStateTransitions.From)
                .Result
                .Map(d => d.Approve(command))
                .Do(res => eventsRepository.AddEvent(res.Event))
                .Match<IActionResult>(
                    Some: result => Ok(result.Document),
                    None: BadRequest);
        }
    }
}