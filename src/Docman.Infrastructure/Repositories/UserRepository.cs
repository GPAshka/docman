using System;
using System.Threading.Tasks;

namespace Docman.Infrastructure.Repositories
{
    public static class UserRepository
    {
        public delegate Task AddUser(Guid userId, string email, string firebaseId);
    }
}