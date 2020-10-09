using LanguageExt;

namespace Docman.Domain.DocumentAggregate
{
    public class FileDescription : NewType<FileDescription, string>
    {
        private FileDescription(string value) : base(value)
        {
        }
        
        public static Validation<Error, Option<FileDescription>> Create(string value)
        {
            if (string.IsNullOrEmpty(value))
                return Option<FileDescription>.None;

            if (value.Length >= 250)
                return new Error("File description should not be larger than 250");

            return Option<FileDescription>.Some(new FileDescription(value));
        }
    }
}