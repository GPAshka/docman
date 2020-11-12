using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Docman.Infrastructure.Dto;
using LanguageExt;

namespace Docman.Infrastructure.Repositories
{
    public static class DocumentRepository
    {
        public delegate Task AddDocument(Guid documentId, string number, string description);
        
        public delegate Task UpdateDocument(Guid documentId, string number, string description);

        public delegate Task UpdateDocumentStatus(Guid documentId, string status);

        public delegate Task ApproveDocument(Guid documentId, string status, string approvalComment);
        
        public delegate Task RejectDocument(Guid documentId, string status, string rejectReason);

        public delegate Task<bool> DocumentExistsByNumber(string number);

        public delegate Task<Option<DocumentDatabaseDto>> GetDocumentById(Guid documentId);

        public delegate Task AddFile(Guid fileId, Guid documentId, string name, string description);

        public delegate Task<Option<FileDatabaseDto>> GetFile(Guid documentId, Guid fileId);
        
        public delegate Task<IEnumerable<FileDatabaseDto>> GetFiles(Guid documentId);
    }
}