namespace Docman.Domain.DocumentAggregate.Errors
{
    public class EmptyValueError : Error
    {
        public EmptyValueError(string fieldName) : base($"{fieldName} should not be empty")
        {
        }
    }
}