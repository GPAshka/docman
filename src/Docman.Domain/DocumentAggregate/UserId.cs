using System;
using LanguageExt;

namespace Docman.Domain.DocumentAggregate
{
    public class UserId : NewType<UserId, Guid>
    {
        public UserId(Guid value) : base(value)
        {
        }
    }
}