using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Docman.API.Application.Commands.Documents;
using Docman.API.Application.Extensions;
using Docman.API.Application.Helpers;
using Docman.Domain;
using Docman.Infrastructure.Repositories;
using LanguageExt;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Docman.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("documents/{documentId:guid}/files")]
    public class DocumentFilesController : DocumentsBaseController
    {
        private readonly DocumentRepository.GetFile _getFile;
        private readonly DocumentRepository.GetFiles _getFiles;

        public DocumentFilesController(
            Func<Guid, Task<Validation<Error, IEnumerable<Event>>>> readEvents,
            Func<Event, Task<Validation<Error, Unit>>> saveAndPublishEventAsync,
            DocumentRepository.GetFile getFile,
            DocumentRepository.GetFiles getFiles,
            Func<HttpContext, Task<Option<Guid>>> getCurrentUserId)
            : base(readEvents, saveAndPublishEventAsync, getCurrentUserId)
        {
            _getFile = getFile;
            _getFiles = getFiles;
        }

        [HttpGet]
        [Route("{fileId:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetFileAsync(Guid documentId, Guid fileId)
        {
            return await _getFile(documentId, fileId)
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
            return await _getFiles(documentId)
                .MapT(ResponseHelper.GenerateFileResponse)
                .Map(Ok);
        }
        
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> AddFile(Guid documentId, [FromBody] AddFileCommand command)
        {
            var outcome = 
                from doc in GetDocumentFromEvents(documentId)
                from result in doc.AddFile(Guid.NewGuid(), command.FileName, command.FileDescription).AsTask()
                from u in ValidateDocumentUser(result.Document, HttpContext)
                from _ in SaveAndPublishEventAsync(result.Event)
                select result.Event;

            return await outcome.Map(val =>
                val.ToActionResult(evt => Created($"documents/{documentId}/files/{evt.FileId}", null)));
        }
    }
}