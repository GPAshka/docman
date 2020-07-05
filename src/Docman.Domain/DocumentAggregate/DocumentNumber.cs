using LanguageExt;

namespace Docman.Domain.DocumentAggregate
{
    public class DocumentNumber : NewType<DocumentNumber, string>
    {
        public DocumentNumber(string value) : base(value)
        {
        }
    }
}