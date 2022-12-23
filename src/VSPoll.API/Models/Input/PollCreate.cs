using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using VSPoll.API.Validations;

namespace VSPoll.API.Models.Input;

public class PollCreate
{
    [Required]
    [MaxLength(100)]
    [Trim]
    public string Description { get; set; } = null!;

    [DefaultValue(VotingSystem.SingleOption)]
    public VotingSystem VotingSystem { get; set; } = VotingSystem.SingleOption;

    [DefaultValue(false)]
    public bool ShowVoters { get; set; }

    [DefaultValue(false)]
    public bool AllowAdd { get; set; }

    public DateTime EndDate { get; set; }

    [Required]
    [MinLength(2)]
    public IEnumerable<string> Options { get; set; } = Enumerable.Empty<string>();
}
