using LanguageExt;

namespace Docman.Domain.Extensions
{
    public static class ErrorExtensions
    {
        public static Error Join(this Seq<Error> errors) => string.Join("; ", errors);
    }
}