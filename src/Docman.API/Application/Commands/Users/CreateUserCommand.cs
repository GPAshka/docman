using LanguageExt;

namespace Docman.API.Application.Commands.Users
{
    public class CreateUserCommand : Record<CreateUserCommand>
    {
        public string Email { get; }
        public string Password { get; }
        
        public CreateUserCommand(string email, string password)
        {
            Email = email;
            Password = password;
        }
    }
}