using System.Collections.Generic;

namespace VSPoll.API.Persistence.Entity
{
    public class PollOption
    {
        public int Id { get; set; }

        public int PollId { get; set; }
        public Poll Poll { get; set; } = null!;

        public string Description { get; set; } = null!;

        public List<PollVote> Votes { get; set; } = new List<PollVote>();
    }
}
