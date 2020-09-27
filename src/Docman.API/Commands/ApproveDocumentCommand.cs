using System;

namespace Docman.API.Commands
{
    public class ApproveDocumentCommand
    {
        public ApproveDocumentCommand(Guid id, string comment)
        {
            Id = id;
            Comment = comment;
        }

        public Guid Id { get; }
        public string Comment { get; }
    }
}