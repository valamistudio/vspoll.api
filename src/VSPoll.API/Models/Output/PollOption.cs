using System;
using VSPoll.API.Utils;
using Entity = VSPoll.API.Persistence.Entities;

namespace VSPoll.API.Models.Output
{
    public class PollOption : IPercentage
    {
        public Guid Id { get; set; }

        public string Description { get; set; } = null!;

        public int Votes { get; set; }

        public decimal Percentage { get; set; }

        public PollOption() { }

        public PollOption(Entity.PollOption option)
        {
            Id = option.Id;
            Description = option.Description;
            Votes = option.Votes.Count;
        }
    }
}
