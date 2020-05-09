using System;

namespace VSPoll.API.Models
{
    public class PollOptionCreate
    {
        public Guid Poll { get; set; }

        public string Description { get; set; } = null!;
    }
}
