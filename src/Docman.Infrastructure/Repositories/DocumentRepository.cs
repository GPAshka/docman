using System;
using System.Threading.Tasks;
using Docman.Domain.DocumentAggregate;
using Docman.Infrastructure.Dto;
using LanguageExt;

namespace Docman.Infrastructure.Repositories
{
    public static class DocumentRepository
    {
        public delegate Task AddDocument(Guid documentId, string number, string description);
        
        public delegate Task UpdateDocument(Guid documentId, string number, string description);

        public delegate Task UpdateDocumentStatus(Guid documentId, string status);

        public delegate Task<bool> DocumentExistsByNumber(string number);

        public delegate Task<Option<DocumentDatabaseDto>> GetDocumentById(Guid documentId);
    }
}