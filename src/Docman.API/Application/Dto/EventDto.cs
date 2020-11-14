using System;
using LanguageExt;

namespace Docman.API.Application.Dto
{
    public class EventDto : Record<EventDto>
    {
        public Guid Id { get; }
        public DateTime TimeStamp { get; }
        
        protected EventDto(Guid id, DateTime timeStamp)
        {
            Id = id;
            TimeStamp = timeStamp;
        }
    }
}