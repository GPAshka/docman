using System;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using Docman.Domain.DocumentAggregate;
using Npgsql;

namespace Docman.Infrastructure.PostgreSql
{
    public static class DocumentPostgresRepository
    {
        public static Func<string, string, string, string, Task> AddDocument =>
            async (connectionString, documentId, number, description) =>
            {
                const string query =
                    "INSERT INTO documents.\"Documents\"(\"Id\", \"Number\", \"Description\", \"Status\") VALUES (@Id, @Number, @Description, @Status)";
                
                using IDbConnection connection = new NpgsqlConnection(connectionString);
                await connection.ExecuteAsync(query, new
                {
                    Id = documentId,
                    Number = number,
                    Description = description,
                    Status = DocumentStatus.Draft.ToString()
                });
            };
    }
}