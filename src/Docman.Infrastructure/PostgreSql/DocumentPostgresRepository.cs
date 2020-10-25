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
        public static Func<string, Guid, string, string, Task> AddDocument =>
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
        
        public static Func<string, Guid, string, string, Task> UpdateDocument =>
            async (connectionString, documentId, number, description) =>
            {
                const string query =
                    "UPDATE documents.\"Documents\" SET \"Number\" = @Number, \"Description\" = @Description WHERE \"Id\" = @Id)";
                
                using IDbConnection connection = new NpgsqlConnection(connectionString);
                await connection.ExecuteAsync(query, new
                {
                    Id = documentId,
                    Number = number,
                    Description = description
                });
            };

        public static Func<string, string, Task<bool>> DocumentExistsByNumberAsync =>
            async (connectionString, number) =>
            {
                const string query = "SELECT COUNT(1) FROM documents.\"Documents\" WHERE \"Number\" = @Number";

                using IDbConnection connection = new NpgsqlConnection(connectionString);
                var exists = await connection.ExecuteScalarAsync<bool>(query, new { Number = number });

                return exists;
            };
    }
}