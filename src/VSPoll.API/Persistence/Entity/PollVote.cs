namespace VSPoll.API.Persistence.Entity
{
    public class PollVote
    {
        public int OptionId { get; set; }
        public PollOption Option { get; set; } = null!;

        public string User { get; set; } = null!;
    }
}
