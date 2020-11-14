using System.Threading;
using System.Threading.Tasks;
using Docman.API.Application.Dto.Events;
using Docman.Infrastructure.Repositories;
using LanguageExt;
using MediatR;
using Microsoft.Extensions.Logging;
using Unit = LanguageExt.Unit;

namespace Docman.API.Application.EventHandlers
{
    public class UpdateDocumentInDatabaseWhenUpdatedEventHandler : INotificationHandler<DocumentUpdatedEventDto>
    {
        private readonly DocumentRepository.UpdateDocument _updateDocument;
        private readonly ILogger<UpdateDocumentInDatabaseWhenUpdatedEventHandler> _logger;

        public UpdateDocumentInDatabaseWhenUpdatedEventHandler(DocumentRepository.UpdateDocument updateDocument,
            ILogger<UpdateDocumentInDatabaseWhenUpdatedEventHandler> logger)
        {
            _updateDocument = updateDocument;
            _logger = logger;
        }

        public async Task Handle(DocumentUpdatedEventDto notification, CancellationToken cancellationToken)
        {
            TryAsync<Unit> handle = async () =>
                await _updateDocument(notification.Id, notification.Number, notification.Description).ToUnit();

            await handle.Match(
                Succ: u => { },
                Fail: ex => _logger.LogError($"Error while handling {nameof(DocumentUpdatedEventDto)}: {ex}"));
        }
    }
}