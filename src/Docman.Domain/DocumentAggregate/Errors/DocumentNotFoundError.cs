using System;

namespace Docman.Domain.DocumentAggregate.Errors
{
    public class DocumentNotFoundError : Error
    {
        public DocumentNotFoundError(Guid documentId) : base($"No document with Id '{documentId}' was found")
        {
        }
    }
}