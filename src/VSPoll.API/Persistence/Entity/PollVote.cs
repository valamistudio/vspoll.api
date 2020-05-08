namespace VSPoll.API.Persistence.Entity
{
    public class PollVote
    {
        public int OptionId { get; set; }
        public PollOption Option { get; set; } = null!;

        public string UserId { get; set; } = null!;
        public User User { get; set; } = null!;
    }
}
