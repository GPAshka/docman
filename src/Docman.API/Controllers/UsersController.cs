using System;
using System.Threading.Tasks;
using Docman.API.Application.Commands.Users;
using Docman.API.Application.Extensions;
using Docman.Domain;
using Docman.Domain.Extensions;
using LanguageExt;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Docman.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly Func<string, string, Task<Validation<Error, string>>> _createFirebaseUser;
        private readonly Func<Event, Task<Validation<Error, Event>>> _saveAndPublishEventAsync;

        public UsersController(Func<string, string, Task<Validation<Error, string>>> createFirebaseUser,
            Func<Event, Task<Validation<Error, Event>>> saveAndPublishEventAsync)
        {
            _createFirebaseUser = createFirebaseUser;
            _saveAndPublishEventAsync = saveAndPublishEventAsync;
        }
        
        [AllowAnonymous]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserCommand command)
        {
            var outcome = from firebaseId in _createFirebaseUser(command.Email, command.Password)
                from evt in command.ToEvent(Guid.NewGuid(), firebaseId).AsTask()
                from _ in _saveAndPublishEventAsync(evt)
                select evt;

            return await outcome.Map(val => val.Match<IActionResult>(
                Succ: evt => Created($"users/{evt.EntityId}", null),
                Fail: errors => BadRequest(new { Errors = errors.Join() })));
        }
    }
}