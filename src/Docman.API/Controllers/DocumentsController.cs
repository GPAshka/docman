using System;
using System.Threading.Tasks;
using Docman.API.Commands;
using Docman.API.Extensions;
using Docman.Domain;
using Docman.Domain.DocumentAggregate;
using Docman.Domain.Events;
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
        public IActionResult CreateDocument([FromBody] CreateDocumentCommand command)
        {
            var eventsRepository = new EventsRepository(new Uri("tcp://admin:changeit@127.0.0.1:1113"));

            void SaveAndPublish(DocumentCreatedEvent evt)
            {
                eventsRepository.AddEvent(evt);
            }

            if (command.Id == Guid.Empty)
                command = command.WithId(Guid.NewGuid());

            return command.ToEvent()
                .Do(SaveAndPublish)
                .Match<IActionResult>(
                    Succ: evt => Created(evt.EntityId.ToString(), null),
                    Fail: errors => BadRequest(string.Join(",", errors)));
        }

        [HttpPost]
        [Route("approve")]
        public async Task<IActionResult> ApproveDocument([FromBody] ApproveDocumentCommand command)
        {
            //TODO validate request
            //TODO get document
            
            var eventsRepository = new EventsRepository(new Uri("tcp://admin:changeit@127.0.0.1:1113"));

            Task<Validation<Error, Document>> getDocument(Guid id) =>
                eventsRepository.ReadEvents(id)
                    .BindT(e => DocumentStates.From(e)
                        .ToValidation(new Error($"No document with Id {id} was found")));

            void SaveAndPublish(Event evt)
            {
                eventsRepository.AddEvent(evt);
            }

            return await getDocument(command.Id)
                .BindT(d => d.Approve(command.Comment))
                .Do(val => 
                    val.Do(res => SaveAndPublish(res.Event)))
                .Map(val => val.Match<IActionResult>(
                    Succ: res => Ok(),
                    Fail: errors => BadRequest(new { Errors = string.Join(",", errors) })));
        }
    }
}