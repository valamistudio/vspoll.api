using System;
using System.Collections.Generic;
using System.Linq;
using VSPoll.API.Utils;
using Entity = VSPoll.API.Persistence.Entities;

namespace VSPoll.API.Models.Output
{
    public class Poll
    {
        public Guid Id { get; set; }

        public string Description { get; set; } = null!;

        public bool MultiVote { get; set; }

        public bool ShowVoters { get; set; }

        public bool AllowAdd { get; set; }

        public DateTime EndDate { get; set; }

        public IEnumerable<PollOption> Options { get; set; } = Enumerable.Empty<PollOption>();

        public Poll() { }

        public Poll(Entity.Poll poll)
        {
            Id = poll.Id;
            Description = poll.Description;
            MultiVote = poll.MultiVote;
            ShowVoters = poll.ShowVoters;
            AllowAdd = poll.AllowAdd;
            EndDate = poll.EndDate;
            Options = poll.Options.Select(option => new PollOption(option)).ToList();
            var totalVotes = Options.Sum(opt => opt.Votes);
            foreach (var option in Options)
                option.Percentage = totalVotes == 0 ? 0 : option.Votes / totalVotes;

            MathUtils.NormalizePercentages(Options);
        }
    }
}
