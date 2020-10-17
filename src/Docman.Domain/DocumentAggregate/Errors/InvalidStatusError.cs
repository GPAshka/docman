namespace Docman.Domain.DocumentAggregate.Errors
{
    public class InvalidStatusError : Error
    {
        public InvalidStatusError(DocumentStatus properStatus, DocumentStatus currentStatus) : base(
            $"Document should have '{properStatus}' status. Current status is '{currentStatus}'")
        {
        }
    }
}