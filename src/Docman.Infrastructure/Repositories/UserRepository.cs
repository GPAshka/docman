using System;
using System.Threading.Tasks;
using Docman.Infrastructure.Dto;
using LanguageExt;

namespace Docman.Infrastructure.Repositories
{
    public static class UserRepository
    {
        public delegate Task AddUser(Guid userId, string email, string firebaseId);

        public delegate Task<Option<UserDatabaseDto>> GetUserByFirebaseId(string firebaseId);
    }
}