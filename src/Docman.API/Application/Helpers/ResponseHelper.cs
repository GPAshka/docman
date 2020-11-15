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
            DocumentDatabaseDto documentDatabaseDto) => new Responses.Document(
            documentDatabaseDto.Id,
            documentDatabaseDto.UserId,
            documentDatabaseDto.Number,
            documentDatabaseDto.Description,
            documentDatabaseDto.Status,
            documentDatabaseDto.ApprovalComment,
            documentDatabaseDto.RejectReason);

        public static Responses.File GenerateFileResponse(FileDatabaseDto fileDatabaseDto) =>
            new Responses.File(
                fileDatabaseDto.Id,
                fileDatabaseDto.DocumentId,
                fileDatabaseDto.Name,
                fileDatabaseDto.Description);

        public static IEnumerable<DocumentHistory> EventsToDocumentHistory(IEnumerable<Event> events) =>
            events
                .Select(ToDocumentHistory)
                .Where(h => h.Status != string.Empty);

        private static DocumentHistory ToDocumentHistory(Event @event)
            => new DocumentHistory(
                GetStatusForEvent(@event),
                @event.TimeStamp);

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