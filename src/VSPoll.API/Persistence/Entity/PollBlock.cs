namespace VSPoll.API.Persistence.Entity
{
    public class PollBlock
    {
        public int PollId { get; set; }
        public Poll Poll { get; set; } = null!;

        public string User { get; set; } = null!;
    }
}
