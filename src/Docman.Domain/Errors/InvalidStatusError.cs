using Docman.Domain.DocumentAggregate;

namespace Docman.Domain.Errors
{
    public class InvalidStatusError : Error
    {
        public InvalidStatusError(DocumentStatus properStatus, DocumentStatus currentStatus) : base(
            $"Document should have '{properStatus}' status. Current status is '{currentStatus}'")
        {
        }
    }
}