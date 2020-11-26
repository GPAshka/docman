namespace Docman.API.Application.Commands.Documents
{
    public class ApproveDocumentCommand
    {
        public ApproveDocumentCommand(string comment)
        {
            Comment = comment;
        }
        
        public string Comment { get; }
    }
}