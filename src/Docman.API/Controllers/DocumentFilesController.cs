using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Docman.API.Application.Commands;
using Docman.API.Application.Helpers;
using Docman.Domain;
using Docman.Domain.DocumentAggregate;
using Docman.Infrastructure.Repositories;
using LanguageExt;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Docman.API.Controllers
{
    [ApiController]
    [Route("documents/{documentId:guid}/files")]
    public class DocumentFilesController : ControllerBase
    {
        private readonly Func<Guid, Task<Validation<Error, IEnumerable<Event>>>> ReadEvents;
        private readonly Action<Event> SaveAndPublishEvent;
        private readonly DocumentRepository.GetFile _getFile;

        public DocumentFilesController(Func<Guid, Task<Validation<Error, IEnumerable<Event>>>> readEvents,
            Action<Event> saveAndPublishEvent, DocumentRepository.GetFile getFile)
        {
            ReadEvents = readEvents;
            SaveAndPublishEvent = saveAndPublishEvent;
            _getFile = getFile;
        }

        private Func<Guid, Task<Validation<Error, Document>>> GetDocumentFromEvents =>
            id => HelperFunctions.GetDocumentFromEvents(ReadEvents, id);
        
        [HttpGet]
        [Route("{fileId:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetFile(Guid documentId, Guid fileId)
        {
            return await _getFile(documentId, fileId)
                .MapT(ResponseHelper.GenerateFileResponse)
                .Map(document =>
                    document.Match<IActionResult>(
                        Some: Ok,
                        None: NotFound()));
        }
        
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AddFile(Guid documentId, [FromBody] AddFileCommand command)
        {
            return await GetDocumentFromEvents(documentId)
                .BindT(d => d.AddFile(command.FileName, command.FileDescription))
                .Do(val =>
                    val.Do(res => SaveAndPublishEvent(res.Event)))
                .Map(val => val.Match<IActionResult>(
                    Succ: res =>
                        Created($"documents/{documentId}/files/{res.Event?.FileId.ToString()}", null),
                    Fail: errors => BadRequest(new { Errors = string.Join(",", errors) })));
        }
    }
}