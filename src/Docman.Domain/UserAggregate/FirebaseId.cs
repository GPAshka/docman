using LanguageExt;

namespace Docman.Domain.UserAggregate
{
    public class FirebaseId : NewType<FirebaseId, string>
    {
        public FirebaseId(string value) : base(value)
        {
        }
    }
}