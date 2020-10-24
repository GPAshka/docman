using System;
using System.Threading.Tasks;
using Docman.Infrastructure.Dto;

namespace Docman.Infrastructure.Repositories
{
    public static class DocumentRepository
    {
        public delegate Task AddDocument(Guid documentId, string number, string description);

        public delegate Task<DocumentDatabaseDto?> GetDocumentByNumber(string name);
    }
}