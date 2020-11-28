using System;

namespace Docman.Domain.Errors
{
    public class FileNotFoundError : Error
    {
        public FileNotFoundError(Guid fileId) : base($"No document with Id '{fileId}' was found")
        {
        }
    }
}