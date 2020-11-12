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
        private readonly Func<Event, Task> SaveAndPublishEvent;
        private readonly DocumentRepository.GetFile GetFile;
        private readonly DocumentRepository.GetFiles GetFiles;

        public DocumentFilesController(Func<Guid, Task<Validation<Error, IEnumerable<Event>>>> readEvents,
            Func<Event, Task> saveAndPublishEvent, DocumentRepository.GetFile getFile, DocumentRepository.GetFiles getFiles)
        {
            ReadEvents = readEvents;
            SaveAndPublishEvent = saveAndPublishEvent;
            GetFile = getFile;
            GetFiles = getFiles;
        }

        private Func<Guid, Task<Validation<Error, Document>>> GetDocumentFromEvents =>
            id => HelperFunctions.GetDocumentFromEvents(ReadEvents, id);
        
        [HttpGet]
        [Route("{fileId:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetFileAsync(Guid documentId, Guid fileId)
        {
            return await GetFile(documentId, fileId)
                .MapT(ResponseHelper.GenerateFileResponse)
                .Map(file =>
                    file.Match<IActionResult>(
                        Some: Ok,
                        None: NotFound()));
        }
        
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetFilesAsync(Guid documentId)
        {
            return await GetFiles(documentId)
                .MapT(ResponseHelper.GenerateFileResponse)
                .Map(Ok);
        }
        
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AddFile(Guid documentId, [FromBody] AddFileCommand command)
        {
            return await GetDocumentFromEvents(documentId)
                .BindT(d => d.AddFile(Guid.NewGuid(), command.FileName, command.FileDescription))
                .Do(val =>
                    val.Do(res => SaveAndPublishEvent(res.Event)))
                .Map(val => val.Match<IActionResult>(
                    Succ: res =>
                        Created($"documents/{documentId}/files/{res.Event?.FileId}", null),
                    Fail: errors => BadRequest(new { Errors = string.Join(",", errors) })));
        }
    }
}