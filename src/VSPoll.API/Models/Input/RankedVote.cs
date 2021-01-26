using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace VSPoll.API.Models.Input
{
    public record RankedVote
    {
        [Required]
        public Authentication User { get; set; } = null!;

        [Required]
        public IEnumerable<Guid> Options { get; set; } = null!;
    }
}
