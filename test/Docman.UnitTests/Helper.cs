using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Docman.Domain;
using LanguageExt;

namespace Docman.UnitTests
{
    public static class Helper
    {
        public static void SaveAndPublish(Event evt) { }

        public static Func<Guid, Task<Validation<Error, IEnumerable<Event>>>> ValidReadEventsFunc(
            params Validation<Error, Event>[] events) => id => Task.FromResult(events.Traverse(x => x));

        public static Func<Guid, Task<Validation<Error, IEnumerable<Event>>>> ReadEventsFuncWithError(string error) =>
            id => Task.FromResult(Validation<Error, IEnumerable<Event>>.Fail(new Seq<Error> { new Error(error) }));
    }
}