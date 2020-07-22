using System;

namespace Docman.API.Requests
{
    public class CreateDocumentRequest
    {
        public Guid DocumentId { get; set; }
        public string DocumentNumber { get; set; }
    }
}