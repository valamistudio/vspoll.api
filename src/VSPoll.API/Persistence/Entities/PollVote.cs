using System;

namespace VSPoll.API.Persistence.Entities;

public class PollVote
{
    public Guid OptionId { get; set; }
    public virtual PollOption Option { get; set; } = null!;

    public int UserId { get; set; }
    public virtual User User { get; set; } = null!;

    public int Weight { get; set; } = 1;

    public DateTime ReferenceDate { get; set; } = DateTime.UtcNow;
}
