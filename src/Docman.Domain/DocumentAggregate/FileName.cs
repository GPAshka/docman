using Docman.Domain.DocumentAggregate.Errors;
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
                return new EmptyValueError("File name");

            return new FileName(value);
        }
    }
}