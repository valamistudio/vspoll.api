using System;
using System.ComponentModel.DataAnnotations;
using VSPoll.API.Validations;

namespace VSPoll.API.Models.Input;

public class PollOptionCreate
{
    [Required]
    public Guid Poll { get; set; }

    [Required]
    [MaxLength(100)]
    [Trim]
    public string Description { get; set; } = null!;
}
