using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Docman.API.Application.Commands;
using Docman.API.Application.Extensions;
using Docman.API.Application.Helpers;
using Docman.API.Application.Responses;
using Docman.Domain;
using Docman.Domain.DocumentAggregate.Errors;
using Docman.Domain.Extensions;
using Docman.Infrastructure.Repositories;
using LanguageExt;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static LanguageExt.Prelude;
using Document = Docman.Domain.DocumentAggregate.Document;

namespace Docman.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class DocumentsController : ControllerBase
    {
        private readonly Func<Guid, Task<Validation<Error, IEnumerable<Event>>>> _readEvents;
        private readonly Func<Event, Task<Validation<Error, Event>>> _saveAndPublishEventAsync;
        private readonly DocumentRepository.DocumentExistsByNumber _documentExistsByNumber;
        private readonly DocumentRepository.GetDocumentById _getDocumentById;

        private Func<Guid, Task<Validation<Error, Document>>> GetDocumentFromEvents =>
            id => DocumentHelper.GetDocumentFromEvents(_readEvents, id);

        private Func<CreateDocumentCommand, Task<Validation<Error, CreateDocumentCommand>>> ValidateCreateCommand =>
            async createCommand =>
            {
                if (await _documentExistsByNumber(createCommand.Number))
                    return new DocumentWithNumberExistsError(createCommand.Number);

                return Validation<Error, CreateDocumentCommand>.Success(createCommand);
            };

        private Func<UpdateDocumentCommand, Task<Validation<Error, UpdateDocumentCommand>>> ValidateUpdateCommand =>
            async updateCommand =>
            {
                if (await _documentExistsByNumber(updateCommand.Number))
                    return new DocumentWithNumberExistsError(updateCommand.Number);

                return Validation<Error, UpdateDocumentCommand>.Success(updateCommand);
            };

        public DocumentsController(Func<Guid, Task<Validation<Error, IEnumerable<Event>>>> readEvents,
            Func<Event, Task<Validation<Error, Event>>> saveAndPublishEventAsync, DocumentRepository.DocumentExistsByNumber documentExistsByNumber,
            DocumentRepository.GetDocumentById getDocumentById)
        {
            _saveAndPublishEventAsync = saveAndPublishEventAsync;
            _readEvents = readEvents;
            _documentExistsByNumber = documentExistsByNumber;
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
            return await _readEvents(documentId)
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
                from _ in _saveAndPublishEventAsync(evt)
                select evt;

            return await outcome.Map(val => val.Match<IActionResult>(
                Succ: evt => Created($"documents/{evt.EntityId}", null),
                Fail: errors => BadRequest(new { Errors = errors.Join() })));
        }

        [HttpPut]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Route("{id:guid}")]
        public async Task<IActionResult> UpdateDocument(Guid id, [FromBody] UpdateDocumentCommand command)
        {
            //More than three from clauses does not work, that's why two of them are combined to local helper function 
            Task<Validation<Error, (UpdateDocumentCommand Command, Document Document)>> ValidateCommandAndGetDocument(
                UpdateDocumentCommand cmd) =>
                from c in ValidateUpdateCommand(cmd)
                from doc in GetDocumentFromEvents(id)
                select (c, doc);

            var outcome =
                from result in ValidateCommandAndGetDocument(command)
                from docEvent in result.Document.Update(result.Command?.Number, result.Command?.Description).AsTask()
                from _ in _saveAndPublishEventAsync(docEvent.Event)
                select docEvent.Document;

            return await outcome.Map(val => val.Match<IActionResult>(
                Succ: _ => NoContent(),
                Fail: errors => BadRequest(new { Errors = errors.Join() })));
        }
        
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Route("{id:guid}/approve")]
        public async Task<IActionResult> ApproveDocument(Guid id, [FromBody] ApproveDocumentCommand command)
        {
            var outcome =
                from doc in GetDocumentFromEvents(id)
                from result in doc.Approve(command.Comment).AsTask()
                from _ in _saveAndPublishEventAsync(result.Event)
                select result.Document;

            return await outcome.Map(val => val.Match<IActionResult>(
                Succ: _ => NoContent(),
                Fail: errors => BadRequest(new { Errors = errors.Join() })));
        }
        
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Route("{id:guid}/reject")]
        public async Task<IActionResult> RejectDocument(Guid id, [FromBody] RejectDocumentCommand command)
        {
            var outcome = 
                from doc in GetDocumentFromEvents(id)
                from result in doc.Reject(command.Reason).AsTask()
                from _ in _saveAndPublishEventAsync(result.Event)
                select result.Document;
            
            return await outcome.Map(val => val.Match<IActionResult>(
                Succ: _ => NoContent(),
                Fail: errors => BadRequest(new { Errors = errors.Join() })));
        }

        [HttpPut]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Route("{id:guid}/send-for-approval")]
        public async Task<IActionResult> SendDocumentForApproval(Guid id)
        {
            var outcome = 
                from doc in GetDocumentFromEvents(id)
                from result in DocumentHelper.SendForApproval(doc).AsTask()
                from _ in _saveAndPublishEventAsync(result.Event)
                select result.Document;

            return await outcome.Map(val => val.Match<IActionResult>(
                Succ: _ => NoContent(),
                Fail: errors => BadRequest(new { Errors = errors.Join() })));
        }
    }
}