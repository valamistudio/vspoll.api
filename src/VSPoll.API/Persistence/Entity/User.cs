using VSPoll.API.Models;

namespace VSPoll.API.Persistence.Entity
{
    public class User
    {
        public int Id { get; set; }

        public string FirstName { get; set; } = null!;

        public string LastName { get; set; } = null!;

        public string Username { get; set; } = null!;

        public string PhotoUrl { get; set; } = null!;

        public User() { }

        public User(Authentication authentication)
        {
            Id = authentication.Id;
            FirstName = authentication.FirstName;
            LastName = authentication.LastName;
            Username = authentication.Username;
            PhotoUrl = authentication.PhotoUrl;
        }
    }
}
