using System.Collections.Generic;
using Docman.Domain.DocumentAggregate;
using Docman.Domain.Events;
using LanguageExt;
using static LanguageExt.Prelude;

namespace Docman.Domain
{
    public static class DocumentStates
    {
        public static Document CreateDocument(this DocumentCreatedEvent evt) 
            => new Document(evt.EntityId, evt.Number, evt.Description, DocumentStatus.Draft);

        public static Document Apply(this Document document, Event evt)
        {
            return evt switch
            {
                DocumentApprovedEvent approvedEvent => new ApprovedDocument(document.Id, document.Number,
                    document.Description, approvedEvent.Comment)
            };
        }

        public static Option<Document> From(IEnumerable<Event> history)
            => history.Match(
                Empty: () => None,
                More: (createdEvent, otherEvents) => Some(
                    otherEvents.Aggregate(
                        seed: CreateDocument((DocumentCreatedEvent) createdEvent),
                        func: (state, evt) => state.Apply(evt)
                    )));

        public static DocumentStatus GetStatus(this Event @event)
            => @event switch
            {
                DocumentCreatedEvent _ => DocumentStatus.Draft,
                DocumentApprovedEvent _ => DocumentStatus.Approved
            };
    }
}