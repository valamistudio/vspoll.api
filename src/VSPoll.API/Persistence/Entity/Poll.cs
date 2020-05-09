using System;
using System.Collections.Generic;
using System.Linq;
using VSPoll.API.Models;

namespace VSPoll.API.Persistence.Entity
{
    public class Poll
    {
        public Guid Id { get; set; }

        public bool MultiVote { get; set; }

        public bool ShowVoters { get; set; } = true;

        public bool AllowAdd { get; set; }

        public DateTime EndDate { get; set; } = DateTime.Now.AddDays(7);

        public List<PollOption> Options { get; set; } = new List<PollOption>();

        public Poll() { }

        public Poll(PollCreate poll)
        {
            AllowAdd = poll.AllowAdd;
            EndDate = poll.EndDate;
            MultiVote = poll.MultiVote;
            Options = poll.Options.Select(option => new PollOption
            {
                Description = option,
            }).ToList();
            ShowVoters = poll.ShowVoters;
        }
    }
}
