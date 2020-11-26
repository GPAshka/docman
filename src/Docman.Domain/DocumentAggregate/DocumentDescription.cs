using System.Runtime.Serialization;
using Docman.Domain.Errors;
using LanguageExt;

namespace Docman.Domain.DocumentAggregate
{
    public class DocumentDescription : NewType<DocumentDescription, string>
    {
        public const int MaxLength = 200;
        
        private DocumentDescription(string value) : base(value)
        {
        }

        public static implicit operator DocumentDescription(string str) => New(str);
        
        public static Validation<Error, Option<DocumentDescription>> Create(string value)
        {
            if (string.IsNullOrEmpty(value))
                return Option<DocumentDescription>.None;

            return value.Length >= MaxLength
                ? new LongValueError("Document description", MaxLength)
                : Validation<Error, Option<DocumentDescription>>.Success(Option<DocumentDescription>.Some(value));
        }
    }
}