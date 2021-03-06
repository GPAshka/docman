namespace Docman.Domain.Errors
{
    public class FileExistsError : Error
    {
        public FileExistsError(string fileName) : base($"Document already has file with name '{fileName}")
        {
        }
    }
}