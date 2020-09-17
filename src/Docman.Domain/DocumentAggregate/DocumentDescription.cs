using System.Runtime.Serialization;
using LanguageExt;

namespace Docman.Domain.DocumentAggregate
{
    public class DocumentDescription : NewType<DocumentDescription, string>
    {
        private DocumentDescription(string value) : base(value)
        {
        }

        public DocumentDescription(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
        
        public static implicit operator DocumentDescription(string str) => New(str);
        
        public static Validation<Error, Option<DocumentDescription>> Create(string value)
        {
            if (string.IsNullOrEmpty(value))
                return Option<DocumentDescription>.None;

            return value.Length >= 100
                ? new Error("Document description should not be larger than 100")
                : Validation<Error, Option<DocumentDescription>>.Success(Option<DocumentDescription>.Some(value));
        }
    }
}