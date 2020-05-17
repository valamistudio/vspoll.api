using System;

namespace VSPoll.API.Persistence.Entity
{
    public class PollVote
    {
        public Guid OptionId { get; set; }
        public virtual PollOption Option { get; set; } = null!;

        public int UserId { get; set; }
        public virtual User User { get; set; } = null!;

        public DateTime ReferenceDate { get; set; } = DateTime.Now;
    }
}
