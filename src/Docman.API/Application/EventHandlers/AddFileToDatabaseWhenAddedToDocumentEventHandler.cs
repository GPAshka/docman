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
    public class AddFileToDatabaseWhenAddedToDocumentEventHandler : INotificationHandler<FileAddedEventDto>
    {
        private readonly ILogger<AddFileToDatabaseWhenAddedToDocumentEventHandler> _logger;
        private readonly DocumentRepository.AddFile _addFile;

        public AddFileToDatabaseWhenAddedToDocumentEventHandler(
            ILogger<AddFileToDatabaseWhenAddedToDocumentEventHandler> logger, DocumentRepository.AddFile addFile)
        {
            _logger = logger;
            _addFile = addFile;
        }

        public async Task Handle(FileAddedEventDto notification, CancellationToken cancellationToken)
        {
            TryAsync<Unit> handle = async () =>
                await _addFile(notification.FileId, notification.Id, notification.FileName,
                    notification.FileDescription).ToUnit();

            await handle.Match(
                Succ: u => { },
                Fail: ex => _logger.LogError($"Error while handling {nameof(FileAddedEventDto)}: {ex}"));
        }
    }
}