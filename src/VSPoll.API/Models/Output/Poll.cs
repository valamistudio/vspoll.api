using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Entity = VSPoll.API.Persistence.Entities;

namespace VSPoll.API.Models.Output;

public class Poll
{
    public Guid Id { get; init; }

    public string Description { get; init; } = null!;

    public VotingSystem VotingSystem { get; init; } = VotingSystem.SingleOption;

    public bool ShowVoters { get; init; }

    public bool AllowAdd { get; init; }

    public DateTime EndDate { get; init; }

    public IEnumerable<PollOption> Options { get; set; } = Enumerable.Empty<PollOption>();

    [return: NotNullIfNotNull(nameof(poll))]
    public static Poll? Of(Entity.Poll? poll)
    {
        if (poll is null)
            return null;

        Poll model = new()
        {
            Id = poll.Id,
            Description = poll.Description,
            VotingSystem = poll.VotingSystem,
            ShowVoters = poll.ShowVoters,
            AllowAdd = poll.AllowAdd,
            EndDate = poll.EndDate,
            //NRT bug when referencing method group
            Options = poll.Options.Select(PollOption.Of)!,
        };
        model.Options = model.Options.ToList();
        var totalVotes = model.Options.Sum(option => option.Votes);
        foreach (var option in model.Options)
            option.Percentage = totalVotes switch
            {
                0 => 0,
                _ => option.Votes / totalVotes,
            };

        return model;
    }
}
