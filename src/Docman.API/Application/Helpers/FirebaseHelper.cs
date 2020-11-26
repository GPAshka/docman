using System;
using System.Threading.Tasks;
using Docman.Domain;
using Firebase.Auth;
using LanguageExt;

namespace Docman.API.Application.Helpers
{
    public static class FirebaseHelper
    {
        public static Func<string, string, string, Task<Validation<Error, string>>> CreateUser =>
            async (firebaseKey, email, password) =>
            {
                return await new TryAsync<string>(async () =>
                {
                    var authProvider = new FirebaseAuthProvider(new FirebaseConfig(firebaseKey));
                    var createUserResult = await authProvider.CreateUserWithEmailAndPasswordAsync(email, password);
                    return createUserResult.User.LocalId;
                }).ToValidation(ex =>
                {
                    return ex switch
                    {
                        FirebaseAuthException authException => new Error(authException.Reason.ToString()),
                        _ => new Error(ex.Message)
                    };
                });
            };
    }
}