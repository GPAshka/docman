namespace Docman.Domain.DocumentAggregate.Errors
{
    public class NoFilesError : Error
    {
        public NoFilesError() : base("Document should have at least one file")
        {
        }
    }
}