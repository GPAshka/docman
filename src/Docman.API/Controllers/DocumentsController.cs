using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Docman.API.Application.Commands;
using Docman.API.Application.Helpers;
using Docman.API.Application.Responses;
using Docman.API.Extensions;
using Docman.Domain;
using Docman.Domain.DocumentAggregate.Errors;
using Docman.Infrastructure.Repositories;
using LanguageExt;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static LanguageExt.Prelude;
using Document = Docman.Domain.DocumentAggregate.Document;

namespace Docman.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DocumentsController : ControllerBase
    {
        private readonly Func<Guid, Task<Validation<Error, IEnumerable<Event>>>> ReadEvents;
        private readonly Func<Event, Task> SaveAndPublishEventAsync;
        private readonly DocumentRepository.DocumentExistsByNumber DocumentExistsByNumber;
        private readonly DocumentRepository.GetDocumentById _getDocumentById;

        private Func<Guid, Task<Validation<Error, Document>>> GetDocumentFromEvents =>
            id => HelperFunctions.GetDocumentFromEvents(ReadEvents, id);

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

        private Func<Event, Task<Validation<Error, Event>>> SaveAndPublishEvent => async e =>
        {
            await SaveAndPublishEventAsync(e);
            return Validation<Error, Event>.Success(e);
        };

        public DocumentsController(Func<Guid, Task<Validation<Error, IEnumerable<Event>>>> readEvents,
            Func<Event, Task> saveAndPublishEventAsync, DocumentRepository.DocumentExistsByNumber documentExistsByNumber,
            DocumentRepository.GetDocumentById getDocumentById)
        {
            SaveAndPublishEventAsync = saveAndPublishEventAsync;
            ReadEvents = readEvents;
            DocumentExistsByNumber = documentExistsByNumber;
            _getDocumentById = getDocumentById;
        }

        [HttpGet]
        [Route("{documentId:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Get(Guid documentId)
        {
            return await _getDocumentById(documentId)
                .MapT(ResponseHelper.GenerateDocumentResponse)
                .Map(document =>
                    document.Match<IActionResult>(
                        Some: Ok,
                        None: NotFound()));
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
                    More: otherEvents => Some(ResponseHelper.EventsToDocumentHistory(otherEvents))))
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
            var outcome =
                from cmd in ValidateCreateCommand(command)
                from evt in cmd.ToEvent(Guid.NewGuid()).AsTask()
                from _ in SaveAndPublishEvent(evt)
                select evt;

            return await outcome.Map(val => val.Match<IActionResult>(
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
                .BindT(c => GetDocumentFromEvents(id)
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
            return await GetDocumentFromEvents(id)
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
            return await GetDocumentFromEvents(id)
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
            return await GetDocumentFromEvents(id)
                .BindT(d => d.SendForApproval())
                .Do(val => val
                    .Do(res => SaveAndPublishEvent(res.Event)))
                .Map(val => val.Match<IActionResult>(
                    Succ: _ => NoContent(),
                    Fail: errors => BadRequest(new { Errors = string.Join(",", errors) })));
        }
    }
}