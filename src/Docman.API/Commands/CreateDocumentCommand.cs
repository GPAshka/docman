using System;

namespace Docman.API.Commands
{
    public class CreateDocumentCommand
    {
        public Guid Id { get; }
        public string Number { get; }
        public string Description { get; }

        public CreateDocumentCommand(Guid id, string number, string description)
        {
            Id = id;
            Number = number;
            Description = description;
        }

        public CreateDocumentCommand WithId(Guid id) => new CreateDocumentCommand(id, Number, Description);
    }
}