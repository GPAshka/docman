using System.Runtime.Serialization;
using Docman.Domain.DocumentAggregate.Errors;
using LanguageExt;

namespace Docman.Domain.DocumentAggregate
{
    public class DocumentNumber : NewType<DocumentNumber, string>
    {
        private DocumentNumber(string value) : base(value)
        {
        }

        public override string ToString() => Value;
        
        public static implicit operator DocumentNumber(string str) => New(str);

        public static Validation<Error, DocumentNumber> Create(string value)
        {
            if (string.IsNullOrEmpty(value))
                return new EmptyValueError("Document number");

            return new DocumentNumber(value);
        }
    }
}