namespace Docman.Domain.Errors
{
    public class DocumentWithNumberExistsError : Error
    {
        public DocumentWithNumberExistsError(string number) : base($"Document with number '{number}' already exists")
        {
        }
    }
}