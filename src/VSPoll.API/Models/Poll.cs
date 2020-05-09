using System;
using System.Collections.Generic;
using System.Linq;
using Entity = VSPoll.API.Persistence.Entity;

namespace VSPoll.API.Models
{
    public class Poll
    {
        public Guid Id { get; set; }

        public bool MultiVote { get; set; }

        public bool ShowVoters { get; set; }

        public bool AllowAdd { get; set; }

        public DateTime EndDate { get; set; }

        public IEnumerable<PollOption> Options { get; set; } = Enumerable.Empty<PollOption>();

        public Poll() { }

        public Poll(Entity.Poll poll)
        {
            Id = poll.Id;
            MultiVote = poll.MultiVote;
            ShowVoters = poll.ShowVoters;
            AllowAdd = poll.AllowAdd;
            EndDate = poll.EndDate;
            Options = poll.Options.Select(option => new PollOption(option));
        }
    }
}
