using Docman.Domain.Errors;
using LanguageExt;

namespace Docman.Domain.DocumentAggregate
{
    public class FileDescription : NewType<FileDescription, string>
    {
        public const int MaxLength = 250;
        
        private FileDescription(string value) : base(value)
        {
        }
        
        public static Validation<Error, Option<FileDescription>> Create(string value)
        {
            if (string.IsNullOrEmpty(value))
                return Option<FileDescription>.None;

            if (value.Length >= MaxLength)
                return new LongValueError("File description", MaxLength);

            return Option<FileDescription>.Some(new FileDescription(value));
        }
    }
}