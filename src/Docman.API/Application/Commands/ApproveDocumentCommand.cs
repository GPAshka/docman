namespace Docman.API.Application.Commands
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