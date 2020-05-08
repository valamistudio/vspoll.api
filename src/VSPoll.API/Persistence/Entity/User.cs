namespace VSPoll.API.Persistence.Entity
{
    public class User
    {
        public string Id { get; set; } = null!;

        public string FirstName { get; set; } = null!;

        public string LastName { get; set; } = null!;

        public string Username { get; set; } = null!;

        public string PhotoUrl { get; set; } = null!;
    }
}
