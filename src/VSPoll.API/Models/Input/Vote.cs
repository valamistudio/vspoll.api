using System;
using System.ComponentModel.DataAnnotations;

namespace VSPoll.API.Models.Input
{
    public class Vote
    {
        [Required]
        public Guid Option { get; set; }

        [Required]
        public Authentication User { get; set; } = null!;
    }
}
