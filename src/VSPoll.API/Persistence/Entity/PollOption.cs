using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using VSPoll.API.Models;

namespace VSPoll.API.Persistence.Entity
{
    public class PollOption
    {
        public Guid Id { get; set; }

        public Guid PollId { get; set; }
        public virtual Poll Poll { get; set; } = null!;

        [MaxLength(100)]
        public string Description { get; set; } = null!;

        public virtual List<PollVote> Votes { get; set; } = new List<PollVote>();

        public PollOption() { }

        public PollOption(PollOptionCreate optionCreate)
        {
            PollId = optionCreate.Poll;
            Description = optionCreate.Description;
        }
    }
}
