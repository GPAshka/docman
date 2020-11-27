using System;

namespace Docman.Infrastructure.Dto
{
    public record UserDatabaseDto
    {
        public Guid Id { get; }
        public string Email { get; }
        public string FirebaseId { get; }
        public DateTime DateCreated { get; }
        
        public UserDatabaseDto(Guid id, string email, string firebaseId, DateTime dateCreated)
        {
            Id = id;
            Email = email;
            FirebaseId = firebaseId;
            DateCreated = dateCreated;
        }
    }
}