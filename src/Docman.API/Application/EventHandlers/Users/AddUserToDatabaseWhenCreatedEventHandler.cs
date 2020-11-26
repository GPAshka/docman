using System.Threading;
using System.Threading.Tasks;
using Docman.API.Application.Dto.UserEvents;
using Docman.Infrastructure.Repositories;
using LanguageExt;
using MediatR;
using Microsoft.Extensions.Logging;
using Unit = LanguageExt.Unit;

namespace Docman.API.Application.EventHandlers.Users
{
    public class AddUserToDatabaseWhenCreatedEventHandler : INotificationHandler<UserCreatedEventDto>
    {
        private readonly ILogger<AddUserToDatabaseWhenCreatedEventHandler> _logger;
        private readonly UserRepository.AddUser _addUser;

        public AddUserToDatabaseWhenCreatedEventHandler(ILogger<AddUserToDatabaseWhenCreatedEventHandler> logger,
            UserRepository.AddUser addUser)
        {
            _logger = logger;
            _addUser = addUser;
        }

        public async Task Handle(UserCreatedEventDto notification, CancellationToken cancellationToken)
        {
            TryAsync<Unit> handle = async () =>
                await _addUser(notification.Id, notification.Email, notification.FirebaseId)
                    .ToUnit();

            await handle.Match(
                Succ: _ => { },
                Fail: ex => _logger.LogError($"Error while handling {nameof(UserCreatedEventDto)}: {ex}"));
        }
    }
}