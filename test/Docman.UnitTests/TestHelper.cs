using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Docman.Domain;
using LanguageExt;
using Microsoft.AspNetCore.Http;

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
    }
}