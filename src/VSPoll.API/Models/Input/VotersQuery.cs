using System;
using System.ComponentModel.DataAnnotations;

namespace VSPoll.API.Models.Input;

public class VotersQuery : Paged
{
    [Required]
    public Guid Option { get; set; }
}
