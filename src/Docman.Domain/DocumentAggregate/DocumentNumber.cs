using System.Runtime.Serialization;
using LanguageExt;

namespace Docman.Domain.DocumentAggregate
{
    public class DocumentNumber : NewType<DocumentNumber, string>
    {
        private DocumentNumber(string value) : base(value)
        {
        }

        public DocumentNumber(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public override string ToString() => Value;
        
        public static implicit operator DocumentNumber(string str) => New(str);

        public static Validation<Error, DocumentNumber> Create(string value)
        {
            if (string.IsNullOrEmpty(value))
                return new Error("Document number should not be empty");

            return new DocumentNumber(value);
        }
    }
}