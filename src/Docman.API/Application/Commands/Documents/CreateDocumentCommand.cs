using System;

namespace Docman.API.Application.Commands.Documents
{
    public class CreateDocumentCommand
    {
        public string Number { get; }
        public string? Description { get; }

        public CreateDocumentCommand(string number, string? description)
        {
            Number = number;
            Description = description;
        }
    }
}