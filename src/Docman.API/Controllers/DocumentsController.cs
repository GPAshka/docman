using System;
using System.Threading.Tasks;
using Docman.API.Commands;
using Docman.Domain;
using Docman.Domain.DocumentAggregate;
using Docman.Domain.Events;
using Docman.Infrastructure.EventStore;
using LanguageExt;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using static LanguageExt.Prelude;

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
                command = command.WithDocumentId(Guid.NewGuid());

            var eventsRepository = new EventsRepository(new Uri("tcp://admin:changeit@127.0.0.1:1113"));
            
            return Success<Error, CreateDocumentCommand>(command)
                .Bind(cmd => Document.Create(cmd.DocumentId, cmd.DocumentNumber))
                .Do(doc =>
                {
                    var evt = new DocumentCreatedEvent(doc.Id, doc.Number);
                    eventsRepository.AddEvent(evt);
                })
                .Match<IActionResult>(
                    Succ: d => Created(d.Id.ToString(), null), 
                    Fail: errors => BadRequest(string.Join(",", errors)));

            // TODO save and publish event
            //return CreatedAtAction(nameof(GetDocument), new {documentId = command.DocumentId}, newState);
        }

        [HttpPost]
        [Route("approve")]
        public async Task<IActionResult> ApproveDocument([FromBody] ApproveDocumentCommand command)
        {
            //TODO validate request
            //TODO get document
            
            var eventsRepository = new EventsRepository(new Uri("tcp://admin:changeit@127.0.0.1:1113"));

            return await eventsRepository.ReadEvents(command.DocumentId)
                .Map(DocumentStates.From)
                .MapT(d => d.Approve())
                .MapT(res => 
                    res.Do(de => eventsRepository.AddEvent(de.Event)))
                .Match(
                    None: NotFound,
                    Some: doc => doc.Match<IActionResult>(
                        Succ: _ => Ok(),
                        Fail: errors => BadRequest(string.Join(",", errors))
                    ));
        }
    }
}