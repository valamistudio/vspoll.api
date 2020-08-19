using System;
using System.ComponentModel.DataAnnotations;

namespace VSPoll.API.Models
{
    public class UserVotes
    {
        [Required]
        public Guid Poll { get; set; }

        [Required]
        public Authentication User { get; set; } = null!;
    }
}
