using System.Threading;
using System.Threading.Tasks;
using Docman.API.Application.Dto.Events;
using Docman.Infrastructure.Repositories;
using MediatR;

namespace Docman.API.Application.EventHandlers
{
    public class AddDocumentToDatabaseWhenCreatedEventHandler : INotificationHandler<DocumentCreatedEventDto>
    {
        private readonly DocumentRepository.AddDocument AddDocument;

        public AddDocumentToDatabaseWhenCreatedEventHandler(DocumentRepository.AddDocument addDocument)
        {
            AddDocument = addDocument;
        }

        public async Task Handle(DocumentCreatedEventDto notification, CancellationToken cancellationToken)
        {
            // TODO handle and log exceptions
            await AddDocument(notification.Id, notification.Number, notification.Description);
        }
    }
}