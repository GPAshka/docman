using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Docman.Infrastructure.Repositories;
using LanguageExt;
using Microsoft.AspNetCore.Http;

namespace Docman.API.Application.Helpers
{
    public static class UserHelper
    {
        public static Func<UserRepository.GetUserByFirebaseId, HttpContext, Task<Option<Guid>>>
            GetCurrentUserId => async (getUserByFirebaseId, context) =>
        {
            //get firebase userId from the token
            var firebaseUserId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var databaseUser = await getUserByFirebaseId(firebaseUserId);
            return databaseUser.Map(user => user.Id);
        };
    }
}