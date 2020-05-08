namespace VSPoll.API.Persistence.Entity
{
    public class PollBlock
    {
        public int PollId { get; set; }
        public Poll Poll { get; set; } = null!;

        public string UserId { get; set; } = null!;
        public User User { get; set; } = null!;
    }
}
