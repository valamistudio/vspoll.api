using VSPoll.API.Models.Input;

namespace VSPoll.API.Persistence.Entities;

public class User
{
    public int Id { get; set; }

    public string FirstName { get; set; } = null!;

    public string? LastName { get; set; }

    public string? Username { get; set; }

    public string? PhotoUrl { get; set; }

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
