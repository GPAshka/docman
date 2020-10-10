using System;
using LanguageExt;

namespace Docman.Domain.DocumentAggregate
{
    public class File
    {
        public Guid Id { get; }
        public FileName Name { get; }
        public Option<FileDescription> Description { get; }
        
        public File(Guid id, FileName name, Option<FileDescription> description)
        {
            Id = id;
            Name = name;
            Description = description;
        }
    }
}