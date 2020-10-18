using System;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using Npgsql;

namespace Docman.Infrastructure.PostgreSql
{
    public static class DocumentPostgreSqlRepository
    {
        public static Func<string, string, string, string, Task> AddDocument =>
            async (connectionString, documentId, number, description) =>
            {
                const string query =
                    "INSERT INTO Documents(Id, Number, Description) VALUES (@Id, @Number, @Description)";
                
                using IDbConnection connection = new NpgsqlConnection(connectionString);
                await connection.ExecuteAsync(query, new
                {
                    Id = documentId,
                    Number = number,
                    Description = description
                });
            };
    }
}