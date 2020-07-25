using LanguageExt;

namespace Docman.Domain
{
    public class Error : NewType<Error, string>
    {
        public Error(string value) : base(value)
        {
        }
        
        public static implicit operator Error(string str) => New(str);
    }
}