using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Docman.API.Commands;
using Docman.API.Extensions;
using Docman.API.Responses;
using Docman.Domain;
using Docman.Domain.DocumentAggregate;
using Docman.Domain.Events;
using LanguageExt;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Docman.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DocumentsController : ControllerBase
    {
        private readonly Func<Guid, Task<Validation<Error, IEnumerable<Event>>>> ReadEvents;
        private readonly Action<Event> SaveAndPublish;

        private Func<Guid, Task<Validation<Error, Document>>> GetDocument => id =>
            ReadEvents(id)
                .BindT(e => DocumentStates.From(e)
                    .ToValidation(new Error($"No document with Id '{id}' was found")));
        
        public DocumentsController(Func<Guid, Task<Validation<Error, IEnumerable<Event>>>> readEvents,
            Action<Event> saveAndPublish)
        {
            SaveAndPublish = saveAndPublish;
            ReadEvents = readEvents;
        }

        [HttpGet]
        [Route("{documentId:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult Get(Guid documentId)
        {
            return Ok();
        }
        
        [HttpGet]
        [Route("{documentId:guid}/history")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<DocumentHistory>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetDocumentHistory(Guid documentId)
        {
            return await ReadEvents(documentId)
                .MapT(e => e.Select(DocumentHistory.EventToDocumentHistory))
                .Map(val => val.Match<IActionResult>(
                    Succ: Ok,
                    Fail: errors => BadRequest(new { Errors = string.Join(",", errors) })));
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult CreateDocument([FromBody] CreateDocumentCommand command)
        {
            if (command.Id == Guid.Empty)
                command = command.WithId(Guid.NewGuid());

            return command.ToEvent()
                .Do(SaveAndPublish)
                .Match<IActionResult>(
                    Succ: evt => Created(evt.EntityId.ToString(), null),
                    Fail: errors => BadRequest(string.Join(",", errors)));
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Route("approve")]
        public async Task<IActionResult> ApproveDocument([FromBody] ApproveDocumentCommand command)
        {
            //TODO validate request

            return await GetDocument(command.Id)
                .BindT(d => d.Approve(command.Comment))
                .Do(val => 
                    val.Do(res => SaveAndPublish(res.Event)))
                .Map(val => val.Match<IActionResult>(
                    Succ: res => NoContent(),
                    Fail: errors => BadRequest(new { Errors = string.Join(",", errors) })));
        }
    }
}