namespace Docman.Domain.Errors
{
    public class LongValueError : Error
    {
        public LongValueError(string fieldName, int maxLength) : base(
            $"{fieldName} should not be larger than {maxLength}")
        {
        }
    }
}