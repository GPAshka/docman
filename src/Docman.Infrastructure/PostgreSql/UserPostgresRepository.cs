using System;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using Docman.Infrastructure.Dto;
using LanguageExt;
using Npgsql;
using static LanguageExt.Prelude;

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

        public static Func<string, string, Task<Option<UserDatabaseDto>>> GetUserByFirebaseId =>
            async (connectionString, firebaseId) =>
            {
                const string query =
                    "SELECT \"Id\", \"Email\", \"FirebaseId\", \"DateCreated\" FROM users.\"Users\" WHERE \"FirebaseId\" = @FirebaseId";
                
                using IDbConnection connection = new NpgsqlConnection(connectionString);
                var user = await connection.QuerySingleOrDefaultAsync<UserDatabaseDto>(query, new { FirebaseId = firebaseId });

                if (user == null)
                    return None;

                return user;
            };
    }
}