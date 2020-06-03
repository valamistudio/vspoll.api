using System;

namespace VSPoll.API.Models
{
    public class UserVotes
    {
        public Guid Poll { get; set; }

        public Authentication User { get; set; } = null!;
    }
}
