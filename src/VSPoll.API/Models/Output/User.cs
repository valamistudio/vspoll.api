using Entity = VSPoll.API.Persistence.Entities;

namespace VSPoll.API.Models.Output;

public class User
{
    public string FirstName { get; init; } = null!;

    public string? LastName { get; init; }

    public string? Username { get; init; }

    public string? PhotoUrl { get; init; }

    public User() { }

    public User(Entity.User user)
    {
        FirstName = user.FirstName;
        LastName = user.LastName;
        Username = user.Username;
        PhotoUrl = user.PhotoUrl;
    }
}
