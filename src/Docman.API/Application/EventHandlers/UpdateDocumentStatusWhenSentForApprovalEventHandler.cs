using System.Threading;
using System.Threading.Tasks;
using Docman.API.Application.Dto.Events;
using Docman.Domain.DocumentAggregate;
using Docman.Infrastructure.Repositories;
using LanguageExt;
using MediatR;
using Microsoft.Extensions.Logging;
using Unit = LanguageExt.Unit;

namespace Docman.API.Application.EventHandlers
{
    public class UpdateDocumentStatusWhenSentForApprovalEventHandler : INotificationHandler<DocumentSentForApprovalEventDto>
    {
        private readonly DocumentRepository.UpdateDocumentStatus _updateDocumentStatus;
        private readonly ILogger<UpdateDocumentStatusWhenSentForApprovalEventHandler> _logger;

        public UpdateDocumentStatusWhenSentForApprovalEventHandler(
            ILogger<UpdateDocumentStatusWhenSentForApprovalEventHandler> logger,
            DocumentRepository.UpdateDocumentStatus updateDocumentStatus)
        {
            _logger = logger;
            _updateDocumentStatus = updateDocumentStatus;
        }

        public async Task Handle(DocumentSentForApprovalEventDto notification, CancellationToken cancellationToken)
        {
            TryAsync<Unit> handle = async () =>
                await _updateDocumentStatus(notification.Id, DocumentStatus.WaitingForApproval.ToString()).ToUnit();

            await handle.Match(
                Succ: u => { },
                Fail: ex => _logger.LogError($"Error while handling {nameof(DocumentSentForApprovalEventDto)}: {ex}"));
        }
    }
}