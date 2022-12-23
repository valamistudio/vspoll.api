using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace VSPoll.API.Models.Output;

public class PollView
{
    public Guid Id { get; init; }

    public string Description { get; init; } = null!;

    public VotingSystem VotingSystem { get; init; }

    public bool ShowVoters { get; init; }

    public bool AllowAdd { get; init; }

    public DateTime EndDate { get; init; }

    public int Voters { get; set; }

    public IEnumerable<PollOption> Options { get; set; } = Enumerable.Empty<PollOption>();

    [return: NotNullIfNotNull(nameof(poll))]
    public static PollView? Of(Poll? poll)
    {
        if (poll is null)
            return null;

        PollView model = new()
        {
            Id = poll.Id,
            Description = poll.Description,
            VotingSystem = poll.VotingSystem,
            ShowVoters = poll.ShowVoters,
            AllowAdd = poll.AllowAdd,
            EndDate = poll.EndDate,
            Options = poll.Options,
        };
        return model;
    }
}
