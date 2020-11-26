using System;
using MediatR;

namespace Docman.API.Application.Dto.Events
{
    public record UserCreatedEventDto : EventDto, INotification
    {
        public string Email { get; }
        public string FirebaseId { get; }
        
        public UserCreatedEventDto(Guid id, DateTime timeStamp, string email, string firebaseId) : base(id, timeStamp)
        {
            Email = email;
            FirebaseId = firebaseId;
        }
    }
}