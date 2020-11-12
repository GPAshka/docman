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
    public class UpdateDocumentInDatabaseWhenRejectedEventHandler : INotificationHandler<DocumentRejectedEventDto>
    {
        private readonly ILogger<UpdateDocumentInDatabaseWhenRejectedEventHandler> _logger;
        private readonly DocumentRepository.RejectDocument _rejectDocument;

        public UpdateDocumentInDatabaseWhenRejectedEventHandler(
            ILogger<UpdateDocumentInDatabaseWhenRejectedEventHandler> logger,
            DocumentRepository.RejectDocument rejectDocument)
        {
            _logger = logger;
            _rejectDocument = rejectDocument;
        }

        public async Task Handle(DocumentRejectedEventDto notification, CancellationToken cancellationToken)
        {
            TryAsync<Unit> handle = async () =>
                await _rejectDocument(notification.Id, DocumentStatus.Rejected.ToString(), notification.Reason)
                    .ToUnit();

            await handle.Match(
                Succ: u => { },
                Fail: ex => _logger.LogError($"Error while handling {nameof(DocumentRejectedEventDto)}: {ex}"));
        }
    }
}