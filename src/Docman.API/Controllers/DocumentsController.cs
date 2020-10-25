using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Docman.API.Application.Commands;
using Docman.API.Application.Helpers;
using Docman.API.Application.Responses;
using Docman.API.Extensions;
using Docman.Domain;
using Docman.Domain.DocumentAggregate;
using Docman.Domain.DocumentAggregate.Errors;
using Docman.Infrastructure.Repositories;
using LanguageExt;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static LanguageExt.Prelude;

namespace Docman.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DocumentsController : ControllerBase
    {
        private readonly Func<Guid, Task<Validation<Error, IEnumerable<Event>>>> ReadEvents;
        private readonly Action<Event> SaveAndPublishEvent;
        private readonly DocumentRepository.DocumentExistsByNumber DocumentExistsByNumber;

        private Func<Guid, Task<Validation<Error, Document>>> GetDocument => id =>
            ReadEvents(id)
                .BindT(events => DocumentHelper.From(events, id));

        private Func<CreateDocumentCommand, Task<Validation<Error, CreateDocumentCommand>>> ValidateCreateCommand =>
            async createCommand =>
            {
                if (await DocumentExistsByNumber(createCommand.Number))
                    return new DocumentWithNumberExistsError(createCommand.Number);

                return Validation<Error, CreateDocumentCommand>.Success(createCommand);
            };

        private Func<UpdateDocumentCommand, Task<Validation<Error, UpdateDocumentCommand>>> ValidateUpdateCommand =>
            async updateCommand =>
            {
                if (await DocumentExistsByNumber(updateCommand.Number))
                    return new DocumentWithNumberExistsError(updateCommand.Number);

                return Validation<Error, UpdateDocumentCommand>.Success(updateCommand);
            };

        public DocumentsController(Func<Guid, Task<Validation<Error, IEnumerable<Event>>>> readEvents,
            Action<Event> saveAndPublishEvent, DocumentRepository.DocumentExistsByNumber documentExistsByNumber)
        {
            SaveAndPublishEvent = saveAndPublishEvent;
            ReadEvents = readEvents;
            DocumentExistsByNumber = documentExistsByNumber;
        }

        [HttpGet]
        [Route("{documentId:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult Get(Guid documentId)
        {
            return Ok();
        }
        
        [HttpGet]
        [Route("{documentId:guid}/files/{fileId:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetFile(Guid documentId, Guid fileId)
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
                .MapT(events => events.Match(
                    Empty: () => None,
                    More: otherEvents => Some(DocumentHistory.EventsToDocumentHistory(otherEvents))))
                .Map(val => val.Match(
                    Fail: errors => BadRequest(string.Join(",", errors)),
                    Succ: res => res.Match<IActionResult>(
                        Some: Ok,
                        None: NotFound)));
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateDocument([FromBody] CreateDocumentCommand command)
        {
            if (command.Id == Guid.Empty)
                command = command.WithId(Guid.NewGuid());

            return await ValidateCreateCommand(command)
                .BindT(c => c.ToEvent())
                .Do(val =>
                    val.Do(e => SaveAndPublishEvent(e)))
                .Map(val => val.Match<IActionResult>(
                    Succ: evt => Created($"documents/{evt.EntityId}", null),
                    Fail: errors => BadRequest(string.Join(",", errors))));
        }

        [HttpPut]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Route("{id:guid}")]
        public async Task<IActionResult> UpdateDocument(Guid id, [FromBody] UpdateDocumentCommand command)
        {
            return await ValidateUpdateCommand(command)
                .BindT(c => GetDocument(id)
                    .BindT(d => d.Update(c.Number, c.Description)))
                .Do(val =>
                    val.Do(res => SaveAndPublishEvent(res.Event)))
                .Map(val => val.Match<IActionResult>(
                    Succ: res => NoContent(),
                    Fail: errors => BadRequest(new { Errors = string.Join(",", errors) })));
        }
        
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Route("{id:guid}/approve")]
        public async Task<IActionResult> ApproveDocument(Guid id, [FromBody] ApproveDocumentCommand command)
        {
            return await GetDocument(id)
                .BindT(d => d.Approve(command.Comment))
                .Do(val => 
                    val.Do(res => SaveAndPublishEvent(res.Event)))
                .Map(val => val.Match<IActionResult>(
                    Succ: res => NoContent(),
                    Fail: errors => BadRequest(new { Errors = string.Join(",", errors) })));
        }
        
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Route("{id:guid}/reject")]
        public async Task<IActionResult> RejectDocument(Guid id, [FromBody] RejectDocumentCommand command)
        {
            return await GetDocument(id)
                .BindT(d => d.Reject(command.Reason))
                .Do(val => 
                    val.Do(res => SaveAndPublishEvent(res.Event)))
                .Map(val => val.Match<IActionResult>(
                    Succ: res => NoContent(),
                    Fail: errors => BadRequest(new { Errors = string.Join(",", errors) })));
        }

        [HttpPut]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Route("{id:guid}/send-for-approval")]
        public async Task<IActionResult> SendDocumentForApproval(Guid id)
        {
            return await GetDocument(id)
                .BindT(d => d.SendForApproval())
                .Do(val => val
                    .Do(res => SaveAndPublishEvent(res.Event)))
                .Map(val => val.Match<IActionResult>(
                    Succ: _ => NoContent(),
                    Fail: errors => BadRequest(new { Errors = string.Join(",", errors) })));
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Route("{id:guid}/files")]
        public async Task<IActionResult> AddFile(Guid id, [FromBody] AddFileCommand command)
        {
            return await GetDocument(id)
                .BindT(d => d.AddFile(command.FileName, command.FileDescription))
                .Do(val =>
                    val.Do(res => SaveAndPublishEvent(res.Event)))
                .Map(val => val.Match<IActionResult>(
                    Succ: res =>
                        Created($"documents/{id}/files/{res.Event?.FileId.ToString()}", null),
                    Fail: errors => BadRequest(new { Errors = string.Join(",", errors) })));
        }
    }
}