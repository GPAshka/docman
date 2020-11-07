using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Docman.API.Application.Commands;
using Docman.API.Application.Helpers;
using Docman.Domain;
using Docman.Domain.DocumentAggregate;
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

        public DocumentFilesController(Func<Guid, Task<Validation<Error, IEnumerable<Event>>>> readEvents,
            Action<Event> saveAndPublishEvent)
        {
            ReadEvents = readEvents;
            SaveAndPublishEvent = saveAndPublishEvent;
        }

        private Func<Guid, Task<Validation<Error, Document>>> GetDocumentFromEvents =>
            id => HelperFunctions.GetDocumentFromEvents(ReadEvents, id);
        
        [HttpGet]
        [Route("{fileId:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetFile(Guid documentId, Guid fileId)
        {
            return Ok();
        }
        
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AddFile(Guid id, [FromBody] AddFileCommand command)
        {
            return await GetDocumentFromEvents(id)
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