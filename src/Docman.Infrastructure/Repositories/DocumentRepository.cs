using System;
using System.Threading.Tasks;

namespace Docman.Infrastructure.Repositories
{
    public static class DocumentRepository
    {
        public delegate Task AddDocument(Guid documentId, string number, string description);
        
        public delegate Task UpdateDocument(Guid documentId, string number, string description);

        public delegate Task<bool> DocumentExistsByNumber(string number);
    }
}