using System;
using System.ComponentModel.DataAnnotations;

namespace VSPoll.API.Models.Input
{
    public class PollOptionCreate
    {
        [Required]
        public Guid Poll { get; set; }

        [Required]
        public string Description { get; set; } = null!;
    }
}
