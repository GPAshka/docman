using System;
using LanguageExt;

namespace Docman.Domain.UserAggregate
{
    public class UserId : NewType<UserId, Guid>
    {
        public UserId(Guid value) : base(value)
        {
        }
    }
}