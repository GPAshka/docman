using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Docman.API.Application.Helpers;
using Docman.Domain;
using Docman.Domain.DocumentAggregate;
using Docman.Domain.Errors;
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

        protected Func<Guid, Task<Validation<Error, Document>>> GetDocumentFromEvents =>
            id => DocumentHelper.GetDocumentFromEvents(ReadEvents, id);
        
        protected Func<Document, HttpContext, Task<Validation<Error, Unit>>> ValidateDocumentUser =>
            async (document, httpContext) =>
            {
                var currentUserId = await GetCurrentUserId(httpContext);
            
                if (document.UserId.Value == currentUserId)
                    return Unit.Default;

                if (httpContext.User.Identity.IsAuthenticated)
                    return new UserForbidError();

                return new UserUnauthorizedError();
            };

        protected DocumentsBaseController(
            Func<Guid, Task<Validation<Error, IEnumerable<Event>>>> readEvents,
            Func<Event, Task<Validation<Error, Unit>>> saveAndPublishEventAsync,
            Func<HttpContext, Task<Option<Guid>>> getCurrentUserId)
        {
            GetCurrentUserId = getCurrentUserId;
            SaveAndPublishEventAsync = saveAndPublishEventAsync;
            ReadEvents = readEvents;
        }
    }
}