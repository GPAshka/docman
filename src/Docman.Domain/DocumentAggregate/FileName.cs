using LanguageExt;

namespace Docman.Domain.DocumentAggregate
{
    public class FileName : NewType<FileName, string>
    {
        private FileName(string value) : base(value)
        {
        }

        public static Validation<Error, FileName> Create(string value)
        {
            if (string.IsNullOrEmpty(value))
                return new Error("File name should not be empty");

            return new FileName(value);
        }
    }
}