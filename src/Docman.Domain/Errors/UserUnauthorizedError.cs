namespace Docman.Domain.Errors
{
    public class UserUnauthorizedError : Error
    {
        public UserUnauthorizedError() : base("Current user is unauthorized")
        {
        }
    }
}