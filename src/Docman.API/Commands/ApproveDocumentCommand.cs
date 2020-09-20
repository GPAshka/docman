using System;

namespace Docman.API.Commands
{
    public class ApproveDocumentCommand
    {
        public Guid Id { get; set; }
        public string Comment { get; set; }
    }
}