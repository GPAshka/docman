namespace Docman.API.Application.Commands
{
    public class RejectDocumentCommand
    {
        public string Reason { get; }
        
        public RejectDocumentCommand(string reason)
        {
            Reason = reason;
        }
    }
}