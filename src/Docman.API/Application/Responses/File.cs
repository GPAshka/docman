using System;

namespace Docman.API.Application.Responses
{
    public class File
    {
        public Guid Id { get; set; }
        public Guid DocumentId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}