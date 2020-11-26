using System.Threading;
using System.Threading.Tasks;
using Docman.API.Application.Dto.DocumentEvents.Events;
using Docman.Domain.DocumentAggregate;
using Docman.Infrastructure.Repositories;
using LanguageExt;
using MediatR;
using Microsoft.Extensions.Logging;
using Unit = LanguageExt.Unit;

namespace Docman.API.Application.EventHandlers.Documents
{
    public class UpdateDocumentInDatabaseWhenApprovedEventHandler : INotificationHandler<DocumentApprovedEventDto>
    {
        private readonly ILogger<UpdateDocumentInDatabaseWhenApprovedEventHandler> _logger;
        private readonly DocumentRepository.ApproveDocument _approveDocument;

        public UpdateDocumentInDatabaseWhenApprovedEventHandler(
            ILogger<UpdateDocumentInDatabaseWhenApprovedEventHandler> logger,
            DocumentRepository.ApproveDocument approveDocument)
        {
            _logger = logger;
            _approveDocument = approveDocument;
        }

        public async Task Handle(DocumentApprovedEventDto notification, CancellationToken cancellationToken)
        {
            TryAsync<Unit> handle = async () =>
                await _approveDocument(notification.Id, DocumentStatus.Approved.ToString(), notification.Comment)
                    .ToUnit();

            await handle.Match(
                Succ: u => { },
                Fail: ex => _logger.LogError($"Error while handling {nameof(DocumentApprovedEventDto)}: {ex}"));
        }
    }
}