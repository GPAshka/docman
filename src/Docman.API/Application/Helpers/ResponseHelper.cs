using System.Collections.Generic;
using System.Linq;
using Docman.API.Application.Responses.Documents;
using Docman.Domain;
using Docman.Domain.DocumentAggregate;
using Docman.Domain.DocumentAggregate.Events;
using Docman.Infrastructure.Dto;
using Document = Docman.API.Application.Responses.Documents.Document;
using File = Docman.API.Application.Responses.Documents.File;

namespace Docman.API.Application.Helpers
{
    public static class ResponseHelper
    {
        public static Document GenerateDocumentResponse(
            DocumentDatabaseDto documentDatabaseDto) => new Document(
            documentDatabaseDto.Id,
            documentDatabaseDto.UserId,
            documentDatabaseDto.Number,
            documentDatabaseDto.Description,
            documentDatabaseDto.Status,
            documentDatabaseDto.ApprovalComment,
            documentDatabaseDto.RejectReason);

        public static File GenerateFileResponse(FileDatabaseDto fileDatabaseDto) =>
            new File(
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