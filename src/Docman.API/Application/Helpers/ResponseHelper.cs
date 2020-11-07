using System.Collections.Generic;
using System.Linq;
using Docman.API.Application.Responses;
using Docman.Domain;
using Docman.Domain.DocumentAggregate;
using Docman.Domain.DocumentAggregate.Events;
using Docman.Infrastructure.Dto;

namespace Docman.API.Application.Helpers
{
    public static class ResponseHelper
    {
        public static Responses.Document GenerateDocumentResponse(
            DocumentDatabaseDto documentDatabaseDto) => new Responses.Document
        {
            Id = documentDatabaseDto.Id,
            Number = documentDatabaseDto.Number,
            Description = documentDatabaseDto.Description,
            Status = documentDatabaseDto.Status,
            ApprovalComment = documentDatabaseDto.ApprovalComment,
            RejectReason = documentDatabaseDto.RejectReason,
        };

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