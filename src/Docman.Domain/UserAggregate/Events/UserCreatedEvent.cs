using System;

namespace Docman.Domain.UserAggregate.Events
{
    public class UserCreatedEvent : Event
    {
        public Email UserEmail { get; }
        public FirebaseId FirebaseId { get; }

        public UserCreatedEvent(UserId entityId, Email userEmail, FirebaseId firebaseId) : base(
            entityId.Value)
        {
            UserEmail = userEmail;
            FirebaseId = firebaseId;
        }

        public UserCreatedEvent(UserId entityId, DateTime timeStamp, Email userEmail, FirebaseId firebaseId) : base(
            entityId.Value, timeStamp)
        {
            UserEmail = userEmail;
            FirebaseId = firebaseId;
        }
    }
}