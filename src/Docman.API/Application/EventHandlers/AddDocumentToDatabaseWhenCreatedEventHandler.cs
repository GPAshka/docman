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
    public class AddDocumentToDatabaseWhenCreatedEventHandler : INotificationHandler<DocumentCreatedEventDto>
    {
        private readonly DocumentRepository.AddDocument _addDocument;
        private readonly ILogger<AddDocumentToDatabaseWhenCreatedEventHandler> _logger;

        public AddDocumentToDatabaseWhenCreatedEventHandler(DocumentRepository.AddDocument addDocument,
            ILogger<AddDocumentToDatabaseWhenCreatedEventHandler> logger)
        {
            _addDocument = addDocument;
            _logger = logger;
        }

        public async Task Handle(DocumentCreatedEventDto notification, CancellationToken cancellationToken)
        {
            TryAsync<Unit> handle = async () =>
                await _addDocument(notification.Id, notification.UserId, notification.Number, notification.Description)
                    .ToUnit();

            await handle.Match(
                Succ: u => { },
                Fail: ex => _logger.LogError($"Error while handling {nameof(DocumentCreatedEventDto)}: {ex}"));
        }
    }
}