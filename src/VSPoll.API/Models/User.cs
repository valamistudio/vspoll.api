using Entity = VSPoll.API.Persistence.Entity;

namespace VSPoll.API.Models
{
    public class User
    {
        public string Id { get; set; } = null!;

        public string FirstName { get; set; } = null!;

        public string LastName { get; set; } = null!;

        public string Username { get; set; } = null!;

        public string PhotoUrl { get; set; } = null!;

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
