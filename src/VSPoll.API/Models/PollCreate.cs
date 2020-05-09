using System;
using System.Collections.Generic;
using System.Linq;

namespace VSPoll.API.Models
{
    public class PollCreate
    {
        public string Description { get; set; } = null!;

        public bool MultiVote { get; set; }

        public bool ShowVoters { get; set; }

        public bool AllowAdd { get; set; }

        public DateTime EndDate { get; set; }

        public IEnumerable<string> Options { get; set; } = Enumerable.Empty<string>();
    }
}
