using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Docman.API.Application.Commands;
using Docman.API.Application.Helpers;
using Docman.Domain;
using Docman.Domain.DocumentAggregate;
using Docman.Domain.Extensions;
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
        private readonly Func<Event, Task> SaveAndPublishEventAsync;
        private readonly DocumentRepository.GetFile GetFile;
        private readonly DocumentRepository.GetFiles GetFiles;

        public DocumentFilesController(Func<Guid, Task<Validation<Error, IEnumerable<Event>>>> readEvents,
            Func<Event, Task> saveAndPublishEventAsync, DocumentRepository.GetFile getFile,
            DocumentRepository.GetFiles getFiles)
        {
            ReadEvents = readEvents;
            SaveAndPublishEventAsync = saveAndPublishEventAsync;
            GetFile = getFile;
            GetFiles = getFiles;
        }

        private Func<Guid, Task<Validation<Error, Document>>> GetDocumentFromEvents =>
            id => HelperFunctions.GetDocumentFromEvents(ReadEvents, id);
        
        private Func<Event, Task<Validation<Error, Event>>> SaveAndPublishEventWithValidation => async evt =>
        {
            await SaveAndPublishEventAsync(evt);
            return Validation<Error, Event>.Success(evt);
        };
        
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
            var outcome = 
                from doc in GetDocumentFromEvents(documentId)
                from result in doc.AddFile(Guid.NewGuid(), command.FileName, command.FileDescription).AsTask()
                from _ in SaveAndPublishEventWithValidation(result.Event)
                select result.Event;

            return await outcome.Map(val => val.Match<IActionResult>(
                Succ: evt =>
                    Created($"documents/{documentId}/files/{evt.FileId}", null),
                Fail: errors => BadRequest(new { Errors = errors.Join() })));
        }
    }
}