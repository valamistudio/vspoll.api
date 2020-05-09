using System;

namespace VSPoll.API.Models
{
    public class Vote
    {
        public Guid Option { get; set; }

        public Authentication User { get; set; } = null!;
    }
}
