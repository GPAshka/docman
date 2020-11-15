using System;
using LanguageExt;

namespace Docman.Infrastructure.Dto
{
    public class FileDatabaseDto : Record<FileDatabaseDto>
    {
        public Guid Id { get; }
        public Guid DocumentId { get; }
        public string Name { get; }
        public string Description { get; }
        
        public FileDatabaseDto(Guid id, Guid documentId, string name, string description)
        {
            Id = id;
            DocumentId = documentId;
            Name = name;
            Description = description;
        }
    }
}