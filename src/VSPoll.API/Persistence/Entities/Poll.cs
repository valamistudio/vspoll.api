using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using VSPoll.API.Models;
using VSPoll.API.Models.Input;

namespace VSPoll.API.Persistence.Entities;

public class Poll
{
    public Guid Id { get; set; }

    [MaxLength(100)]
    public string Description { get; set; } = null!;

    public VotingSystem VotingSystem { get; set; } = VotingSystem.SingleOption;

    public bool ShowVoters { get; set; } = true;

    public bool AllowAdd { get; set; }

    public DateTime EndDate { get; set; } = DateTime.UtcNow.AddDays(7);

    public virtual List<PollOption> Options { get; set; } = new();

    public Poll() { }

    public Poll(PollCreate poll)
    {
        Description = poll.Description;
        AllowAdd = poll.AllowAdd;
        EndDate = poll.EndDate;
        VotingSystem = poll.VotingSystem;
        Options = poll.Options.Select(option => new PollOption
        {
            Description = option,
        }).ToList();
        ShowVoters = poll.ShowVoters;
    }
}
