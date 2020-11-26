using Docman.Domain.Errors;
using LanguageExt;

namespace Docman.Domain.UserAggregate
{
    public class Email : NewType<Email, string>
    {
        private Email(string value) : base(value)
        {
        }

        public static Validation<Error, Email> Create(string value)
        {
            if (!System.Net.Mail.MailAddress.TryCreate(value, out _))
                return new InvalidEmailError(value);

            return new Email(value);
        }
    }
}