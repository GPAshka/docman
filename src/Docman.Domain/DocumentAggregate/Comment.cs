using LanguageExt;

namespace Docman.Domain.DocumentAggregate
{
    public class Comment : NewType<Comment, string>
    {
        private Comment(string value) : base(value)
        {
        }
        
        public static Validation<Error, Comment> Create(string value)
        {
            if (string.IsNullOrEmpty(value))
                return new Error("Comment should not be empty");

            return new Comment(value);
        }
    }
}