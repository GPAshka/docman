using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Docman.Domain;
using Docman.Infrastructure.Dto;
using Docman.Infrastructure.Repositories;
using LanguageExt;
using Microsoft.AspNetCore.Http;
using static LanguageExt.Prelude;

namespace Docman.UnitTests
{
    public static class TestHelper
    {
        public static Task<Validation<Error, Unit>> SaveAndPublish(Event evt)
        {
            return Task.FromResult(Validation<Error, Unit>.Success(Unit.Default));
        }

        public static Func<Guid, Task<Validation<Error, IEnumerable<Event>>>> ValidReadEventsFunc(
            params Validation<Error, Event>[] events) => _ => Task.FromResult(events.Traverse(x => x));

        public static Func<Guid, Task<Validation<Error, IEnumerable<Event>>>> ReadEventsFuncWithError(string error) =>
            _ => Task.FromResult(Validation<Error, IEnumerable<Event>>.Fail(new Seq<Error> { new(error) }));

        public static Func<HttpContext, Task<Option<Guid>>> GetCurrentUserId() =>
            _ => Task.FromResult<Option<Guid>>(Guid.Empty);
        
        public static DocumentRepository.GetDocumentById GetDocumentById() =>
            documentId => Task.FromResult(Some(new DocumentDatabaseDto(documentId, Guid.Empty, string.Empty,
                string.Empty, string.Empty, null, null, DateTime.UtcNow)));
    }
}