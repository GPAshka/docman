using Docman.API.Commands;
using Docman.API.Extensions;
using Docman.Domain;
using Docman.Domain.DocumentAggregate;
using Docman.Domain.Events;

namespace Docman.API.CommandHandlers
{
    public static class DocumentCommandHandlers
    {
        public static (Event Event, Document Document) Create(CreateDocumentCommand command)
        {
            var evt = command.ToEvent();
            var newState = evt.CreateDocument();

            return ((Event) evt, newState);
        }

        public static (Event Event, Document Document) Approve(this Document document, ApproveDocumentCommand command)
        {
            var evt = command.ToEvent();
            var newState = document.Apply(evt);

            return ((Event) evt, newState);
        }
    }
}