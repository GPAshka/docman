namespace Docman.Domain.Errors
{
    public class InvalidEmailError : Error
    {
        public InvalidEmailError(string value) : base($"{value} is not valid Email address")
        {
        }
    }
}