using System;
using System.Collections.Generic;
using System.Linq;
using VSPoll.API.Utils;
using Entity = VSPoll.API.Persistence.Entities;

namespace VSPoll.API.Models.Output
{
    public class Poll
    {
        public Guid Id { get; init; }

        public string Description { get; init; } = null!;

        public bool MultiVote { get; init; }

        public bool ShowVoters { get; init; }

        public bool AllowAdd { get; init; }

        public DateTime EndDate { get; init; }

        public IEnumerable<PollOption> Options { get; init; } = Enumerable.Empty<PollOption>();

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
