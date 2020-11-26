using System;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using Npgsql;

namespace Docman.Infrastructure.PostgreSql
{
    public static class UserPostgresRepository
    {
        public static Func<string, Guid, string, string, Task> Adduser =>
            async (connectionString, userId, email, firebaseId) =>
            {
                const string query =
                    "INSERT INTO users.\"Users\"(\"Id\", \"Email\", \"FirebaseId\") VALUES (@Id, @Email, @FirebaseId)";
                
                using IDbConnection connection = new NpgsqlConnection(connectionString);
                await connection.ExecuteAsync(query, new
                {
                    Id = userId,
                    Email = email,
                    FirebaseId = firebaseId
                });
            };
    }
}