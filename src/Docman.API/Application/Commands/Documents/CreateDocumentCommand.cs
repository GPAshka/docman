using System;

namespace Docman.API.Application.Commands.Documents
{
    public class CreateDocumentCommand
    {
        public Guid UserId { get; }
        public string Number { get; }
        public string? Description { get; }

        public CreateDocumentCommand(Guid userId, string number, string description)
        {
            UserId = userId;
            Number = number;
            Description = description;
        }
    }
}