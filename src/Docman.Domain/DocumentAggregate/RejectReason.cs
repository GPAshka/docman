using LanguageExt;

namespace Docman.Domain.DocumentAggregate
{
    public class RejectReason : NewType<RejectReason, string>
    {
        public RejectReason(string value) : base(value)
        {
        }
        
        public static Validation<Error, RejectReason> Create(string value)
        {
            if (string.IsNullOrEmpty(value))
                return new Error("Reject reason should not be empty");

            return new RejectReason(value);
        }
    }
}