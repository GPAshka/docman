using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using Docman.Domain.DocumentAggregate;
using Docman.Infrastructure.Dto;
using LanguageExt;
using Npgsql;
using static LanguageExt.Prelude;

namespace Docman.Infrastructure.PostgreSql
{
    public static class DocumentPostgresRepository
    {
        public static Func<string, Guid, Guid, string, string, Task> AddDocument =>
            async (connectionString, documentId, userId, number, description) =>
            {
                const string query =
                    "INSERT INTO documents.\"Documents\"(\"Id\", \"UserId\", \"Number\", \"Description\", \"Status\") VALUES (@Id, @UserId, @Number, @Description, @Status)";
                
                using IDbConnection connection = new NpgsqlConnection(connectionString);
                await connection.ExecuteAsync(query, new
                {
                    Id = documentId,
                    UserId = userId,
                    Number = number,
                    Description = description,
                    Status = DocumentStatus.Draft.ToString()
                });
            };
        
        public static Func<string, Guid, string, string, Task> UpdateDocument =>
            async (connectionString, documentId, number, description) =>
            {
                const string query =
                    "UPDATE documents.\"Documents\" SET \"Number\" = @Number, \"Description\" = @Description WHERE \"Id\" = @Id";
                
                using IDbConnection connection = new NpgsqlConnection(connectionString);
                await connection.ExecuteAsync(query, new
                {
                    Id = documentId,
                    Number = number,
                    Description = description
                });
            };

        public static Func<string, Guid, string, string, Task> ApproveDocument =>
            async (connectionString, documentId, status, approvalComment) =>
            {
                const string query =
                    "UPDATE documents.\"Documents\" SET \"Status\" = @Status, \"ApprovalComment\" = @ApprovalComment WHERE \"Id\" = @Id";
                
                using IDbConnection connection = new NpgsqlConnection(connectionString);
                await connection.ExecuteAsync(query, new
                {
                    Id = documentId,
                    Status = status,
                    ApprovalComment = approvalComment,
                });
            };
        
        public static Func<string, Guid, string, string, Task> RejectDocument =>
            async (connectionString, documentId, status, rejectReason) =>
            {
                const string query =
                    "UPDATE documents.\"Documents\" SET \"Status\" = @Status, \"RejectReason\" = @RejectReason WHERE \"Id\" = @Id";
                
                using IDbConnection connection = new NpgsqlConnection(connectionString);
                await connection.ExecuteAsync(query, new
                {
                    Id = documentId,
                    Status = status,
                    RejectReason = rejectReason,
                });
            };
        
        public static Func<string, Guid, string, Task> UpdateDocumentStatus =>
            async (connectionString, documentId, status) =>
            {
                const string query = "UPDATE documents.\"Documents\" SET \"Status\" = @Status WHERE \"Id\" = @Id";
                
                using IDbConnection connection = new NpgsqlConnection(connectionString);
                await connection.ExecuteAsync(query, new
                {
                    Id = documentId,
                    Status = status
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

        public static Func<string, Guid, Task<Option<DocumentDatabaseDto>>> GetDocumentByIdAsync =>
            async (connectionString, documentId) =>
            {
                const string query =
                    "SELECT \"Id\", \"UserId\", \"Number\", \"Description\", \"Status\", \"ApprovalComment\", \"RejectReason\", \"DateCreated\" FROM documents.\"Documents\" WHERE \"Id\" = @Id";
                
                using IDbConnection connection = new NpgsqlConnection(connectionString);
                var document =
                    await connection.QuerySingleOrDefaultAsync<DocumentDatabaseDto>(query, new { Id = documentId });

                if (document == null)
                    return None;

                return document;
            };
        
        public static Func<string, Guid, Guid, string, string, Task> AddFile =>
            async (connectionString, fileId, documentId, name, description) =>
            {
                const string query =
                    "INSERT INTO documents.\"Files\"(\"Id\", \"DocumentId\", \"Name\", \"Description\") VALUES (@Id, @DocumentId, @Name, @Description)";
                
                using IDbConnection connection = new NpgsqlConnection(connectionString);
                await connection.ExecuteAsync(query, new
                {
                    Id = fileId,
                    DocumentId = documentId,
                    Name = name,
                    Description = description
                });
            };
        
        public static Func<string, Guid, Guid, Task<Option<FileDatabaseDto>>> GetFileByIdAsync =>
            async (connectionString, documentId, fileId) =>
            {
                const string query =
                    "SELECT \"Id\", \"DocumentId\", \"Name\", \"Description\" FROM documents.\"Files\" WHERE \"Id\" = @Id AND \"DocumentId\" = @DocumentId";
                
                using IDbConnection connection = new NpgsqlConnection(connectionString);
                var file = await connection.QuerySingleOrDefaultAsync<FileDatabaseDto>(query,
                    new { Id = fileId, DocumentId = documentId });

                if (file == null)
                    return None;

                return file;
            };
        
        public static Func<string, Guid, Task<IEnumerable<FileDatabaseDto>>> GetFilesAsync =>
            async (connectionString, documentId) =>
            {
                const string query =
                    "SELECT \"Id\", \"DocumentId\", \"Name\", \"Description\" FROM documents.\"Files\" WHERE \"DocumentId\" = @DocumentId";
                
                using IDbConnection connection = new NpgsqlConnection(connectionString);
                var files = await connection.QueryAsync<FileDatabaseDto>(query, new { DocumentId = documentId });

                return files;
            };
    }
}