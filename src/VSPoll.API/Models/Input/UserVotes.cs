using System;
using System.ComponentModel.DataAnnotations;

namespace VSPoll.API.Models.Input
{
    public class UserVotes
    {
        [Required]
        public Guid Poll { get; set; }

        [Required]
        public Authentication User { get; set; } = null!;
    }
}
