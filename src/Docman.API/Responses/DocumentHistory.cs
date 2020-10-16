using System;
using System.Collections.Generic;
using System.Linq;
using Docman.Domain;
using Docman.Domain.DocumentAggregate;
using Docman.Domain.Events;

namespace Docman.API.Responses
{
    public class DocumentHistory
    {
        public string Status { get; set; }
        public DateTime TimeStamp { get; set; }

        public static IEnumerable<DocumentHistory> EventsToDocumentHistory(IEnumerable<Event> events) =>
            events
                .Select(ToDocumentHistory)
                .Where(h => h.Status != string.Empty);


        private static DocumentHistory ToDocumentHistory(Event @event)
            => new DocumentHistory
            {
                Status = GetStatusForEvent(@event),
                TimeStamp = @event.TimeStamp
            };
        
        private static string GetStatusForEvent(Event @event)
            => @event switch
            {
                DocumentCreatedEvent _ => DocumentStatus.Draft.ToString(),
                DocumentSentForApprovalEvent _ => DocumentStatus.WaitingForApproval.ToString(),
                DocumentApprovedEvent _ => DocumentStatus.Approved.ToString(),
                DocumentRejectedEvent _ => DocumentStatus.Rejected.ToString(),
                _ => string.Empty
            };
    }
}