namespace VSPoll.API.Models
{
    public class Authentication
    {
        public int Id { get; set; }

        public string FirstName { get; set; } = null!;

        public string? LastName { get; set; }

        public string? Username { get; set; }

        public string? PhotoUrl { get; set; }

        public long AuthDate { get; set; }

        public string Hash { get; set; } = null!;
    }
}
