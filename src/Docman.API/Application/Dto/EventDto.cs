using System;

namespace Docman.API.Application.Dto
{
    public record EventDto
    {
        public Guid Id { get; }
        public DateTime TimeStamp { get; }

        protected EventDto(Guid id, DateTime timeStamp) => (Id, TimeStamp) = (id, timeStamp);
    }
}