using Entity = VSPoll.API.Persistence.Entities;

namespace VSPoll.API.Models
{
    public class User
    {
        public int Id { get; set; }

        public string FirstName { get; set; } = null!;

        public string? LastName { get; set; }

        public string? Username { get; set; }

        public string? PhotoUrl { get; set; }

        public User() { }

        public User(Entity.User user)
        {
            Id = user.Id;
            FirstName = user.FirstName;
            LastName = user.LastName;
            Username = user.Username;
            PhotoUrl = user.PhotoUrl;
        }
    }
}
