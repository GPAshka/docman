namespace Docman.API.Application.Commands.Documents
{
    public class UpdateDocumentCommand
    {
        public UpdateDocumentCommand(string number, string description)
        {
            Number = number;
            Description = description;
        }

        public string Number { get; }
        public string Description { get; }
    }
}