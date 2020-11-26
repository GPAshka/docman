using System;
using LanguageExt;

namespace Docman.API.Application.Responses.Documents
{
    public class File : Record<File>
    {
        public Guid Id { get; }
        public Guid DocumentId { get; }
        public string Name { get; }
        public string Description { get; }
        
        public File(Guid id, Guid documentId, string name, string description)
        {
            Id = id;
            DocumentId = documentId;
            Name = name;
            Description = description;
        }
    }
}