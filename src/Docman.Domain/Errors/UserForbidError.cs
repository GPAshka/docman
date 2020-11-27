namespace Docman.Domain.Errors
{
    public class UserForbidError : Error
    {
        public UserForbidError() : base("Access forbidden for the current user")
        {
        }
    }
}