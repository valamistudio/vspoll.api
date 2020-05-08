using System.Collections.Generic;
using System.Linq;

namespace VSPoll.API.Models
{
    public class PollOption
    {
        public int Id { get; set; }

        public string Description { get; set; } = null!;

        public int Votes { get; set; }

        public decimal Percentage { get; set; }

        public IEnumerable<User> Voters { get; set; } = Enumerable.Empty<User>();
    }
}
