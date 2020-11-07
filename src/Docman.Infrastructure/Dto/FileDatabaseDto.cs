using System;

namespace Docman.Infrastructure.Dto
{
    public class FileDatabaseDto
    {
        public Guid Id { get; set; }
        public Guid DocumentId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}