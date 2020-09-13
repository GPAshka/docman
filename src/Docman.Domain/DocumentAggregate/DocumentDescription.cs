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
        
        public static Validation<Error, DocumentDescription> Create(string value)
        {
            if (string.IsNullOrEmpty(value))
                return new Error("Document description should not be empty");

            return new DocumentDescription(value);
        }
    }
}