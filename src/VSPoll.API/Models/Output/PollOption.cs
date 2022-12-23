using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using VSPoll.API.Utils;
using Entity = VSPoll.API.Persistence.Entities;

namespace VSPoll.API.Models.Output;

public class PollOption : IPercentage
{
    public Guid Id { get; init; }

    public Guid Poll { get; init; }

    public string Description { get; init; } = null!;

    public int Votes { get; init; }

    public decimal Percentage { get; set; }

    [return: NotNullIfNotNull(nameof(option))]
    public static PollOption? Of(Entity.PollOption? option) => option is null ? null : new()
    {
        Id = option.Id,
        Description = option.Description,
        Poll = option.PollId,
        Votes = option.Votes.Sum(vote => vote.Weight),
    };
}
