using System;
using System.Collections.Generic;
using System.Linq;
using Entity = VSPoll.API.Persistence.Entity;

namespace VSPoll.API.Models
{
    public class PollOption
    {
        public Guid Id { get; set; }

        public string Description { get; set; } = null!;

        public int Votes { get; set; }

        public decimal Percentage { get; set; }

        public IEnumerable<User> Voters { get; set; } = Enumerable.Empty<User>();

        public PollOption() { }

        public PollOption(Entity.PollOption option)
        {
            Id = option.Id;
            Description = option.Description;
            Votes = option.Votes.Count;
            //ToDo: normalize
            var totalVotes = option.Poll.Options.Sum(opt => opt.Votes.Count);
            Percentage = totalVotes == 0 ? 0 : option.Votes.Count / totalVotes;
            Voters = option.Votes.Select(vote => new User(vote.User));
        }
    }
}
