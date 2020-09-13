using System;
using System.Threading.Tasks;
using Docman.API.Commands;
using Docman.Domain;
using Docman.Domain.DocumentAggregate;
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

            return command.ToEvent()
                .Map(evt => (evt.CreateDocument(), evt))
                .Do(res => eventsRepository.AddEvent(res.evt))
                .Match<IActionResult>(
                    Succ: res => Created(res.Item1.Id.ToString(), null),
                    Fail: errors => BadRequest(string.Join(",", errors)));;
            
            /*return Success<Error, CreateDocumentCommand>(command)
                .Bind(cmd => Document.Create(cmd.DocumentId, cmd.Number, cmd.Description))
                .Do(res => eventsRepository.AddEvent(res.Event)) //TODO publish event
                .Match<IActionResult>(
                    Succ: res => Created(res.Document.Id.ToString(), null),
                    Fail: errors => BadRequest(string.Join(",", errors)));*/
        }

        [HttpPost]
        [Route("approve")]
        public async Task<IActionResult> ApproveDocument([FromBody] ApproveDocumentCommand command)
        {
            //TODO validate request
            //TODO get document
            
            var eventsRepository = new EventsRepository(new Uri("tcp://admin:changeit@127.0.0.1:1113"));

            Task<Validation<Error, Document>> GetDocument(Guid id) =>
                eventsRepository.ReadEvents(id)
                    .Map(DocumentStates.From)
                    .Map(opt => opt.ToValidation(new Error($"No document with Id {command.DocumentId} was found")));

            return await GetDocument(command.DocumentId)
                .BindT(d => d.Approve())
                .Map(val =>
                    val.Do(res => eventsRepository.AddEvent(res.Event)))
                .Map(val => val.Match<IActionResult>(
                    Succ: res => Ok(),
                    Fail: errors => BadRequest(new { Errors = string.Join(",", errors) })));
        }
    }
}