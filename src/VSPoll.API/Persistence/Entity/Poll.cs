using System;
using System.Collections.Generic;

namespace VSPoll.API.Persistence.Entity
{
    public class Poll
    {
        public int Id { get; set; }

        public bool MultiVote { get; set; }

        public bool ShowVoters { get; set; } = true;

        public bool AllowAdd { get; set; }

        public DateTime EndDate { get; set; } = DateTime.Now.AddDays(7);

        public string? UserId { get; set; }
        public User? User { get; set; }

        public List<PollBlock> Blocks { get; set; } = new List<PollBlock>();
        public List<PollOption> Options { get; set; } = new List<PollOption>();
    }
}
