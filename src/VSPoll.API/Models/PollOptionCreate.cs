using System;
using System.ComponentModel.DataAnnotations;

namespace VSPoll.API.Models
{
    public class PollOptionCreate
    {
        [Required]
        public Guid Poll { get; set; }

        [Required]
        public string Description { get; set; } = null!;
    }
}
