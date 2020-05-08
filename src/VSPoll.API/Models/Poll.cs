using System;
using System.Collections.Generic;
using System.Linq;

namespace VSPoll.API.Models
{
    public class Poll
    {
        public bool MultiVote { get; set; }

        public bool ShowVoters { get; set; }

        public bool AllowAdd { get; set; }

        public DateTime EndDate { get; set; }

        public string? User { get; set; }

        public UserType UserType { get; set; }

        public IEnumerable<PollOption> Options { get; set; } = Enumerable.Empty<PollOption>();
    }
}
