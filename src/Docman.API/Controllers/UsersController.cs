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
        private readonly Func<string, string, Task<Validation<Error, string>>> _signInUser;
        private readonly Func<Event, Task<Validation<Error, Unit>>> _saveAndPublishEventAsync;

        public UsersController(Func<string, string, Task<Validation<Error, string>>> createFirebaseUser,
            Func<Event, Task<Validation<Error, Unit>>> saveAndPublishEventAsync,
            Func<string, string, Task<Validation<Error, string>>> signInUser)
        {
            _createFirebaseUser = createFirebaseUser;
            _saveAndPublishEventAsync = saveAndPublishEventAsync;
            _signInUser = signInUser;
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

        [AllowAnonymous]
        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> SignIn([FromBody] CreateUserCommand command)
        {
            return await _signInUser(command.Email, command.Password)
                .Map(val => val.Match<IActionResult>(
                    Succ: token => Ok(new { AccessToken = token }),
                    Fail: errors => BadRequest(new { Errors = errors.Join() })));
        }
    }
}