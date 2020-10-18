using System.Threading.Tasks;

namespace Docman.Infrastructure.Repositories
{
    public static class DocumentRepository
    {
        public delegate Task AddDocument(string documentId, string number, string description);
    }
}