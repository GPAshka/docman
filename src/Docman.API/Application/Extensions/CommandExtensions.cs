using System;
using Docman.API.Application.Commands.Documents;
using Docman.API.Application.Commands.Users;
using Docman.Domain;
using Docman.Domain.DocumentAggregate;
using Docman.Domain.DocumentAggregate.Events;
using Docman.Domain.UserAggregate;
using Docman.Domain.UserAggregate.Events;
using LanguageExt;

namespace Docman.API.Application.Extensions
{
    public static class CommandExtensions
    {
        public static Validation<Error, Event>
            ToEvent(this CreateDocumentCommand command, Guid documentId, Guid userId) =>
            (DocumentNumber.Create(command.Number), DocumentDescription.Create(command.Description)).Apply(
                (num, desc) => (Event) new DocumentCreatedEvent(new DocumentId(documentId),
                    new Domain.DocumentAggregate.UserId(userId), num, desc));

        public static Validation<Error, Event> ToEvent(this CreateUserCommand command, Guid userId,
            string userFirebaseId) =>
            Email.Create(command.Email)
                .Map(email => (Event) new UserCreatedEvent(new Domain.UserAggregate.UserId(userId), email,
                    new FirebaseId(userFirebaseId)));
    }
}