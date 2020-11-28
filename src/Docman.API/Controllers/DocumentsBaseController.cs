using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Docman.API.Application.Helpers;
using Docman.Domain;
using Docman.Domain.DocumentAggregate;
using Docman.Domain.Errors;
using Docman.Infrastructure.Repositories;
using LanguageExt;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Docman.API.Controllers
{
    public abstract class DocumentsBaseController : ControllerBase
    {
        protected readonly Func<Guid, Task<Validation<Error, IEnumerable<Event>>>> ReadEvents;
        protected readonly Func<Event, Task<Validation<Error, Unit>>> SaveAndPublishEventAsync;
        protected readonly Func<HttpContext, Task<Option<Guid>>> GetCurrentUserId;
        protected readonly DocumentRepository.GetDocumentById GetDocumentById;

        protected Func<Guid, Task<Validation<Error, Document>>> GetDocumentFromEvents =>
            id => DocumentHelper.GetDocumentFromEvents(ReadEvents, id);
        
        protected Func<Guid, Task<Validation<Error, Unit>>> ValidateDocumentUser =>
            async userId =>
            {
                var currentUserId = await GetCurrentUserId(HttpContext);
            
                if (userId == currentUserId)
                    return Unit.Default;

                if (HttpContext.User.Identity.IsAuthenticated)
                    return new UserForbidError();

                return new UserUnauthorizedError();
            };

        protected DocumentsBaseController(
            Func<Guid, Task<Validation<Error, IEnumerable<Event>>>> readEvents,
            Func<Event, Task<Validation<Error, Unit>>> saveAndPublishEventAsync,
            Func<HttpContext, Task<Option<Guid>>> getCurrentUserId, 
            DocumentRepository.GetDocumentById getDocumentById)
        {
            GetCurrentUserId = getCurrentUserId;
            GetDocumentById = getDocumentById;
            SaveAndPublishEventAsync = saveAndPublishEventAsync;
            ReadEvents = readEvents;
        }
    }
}