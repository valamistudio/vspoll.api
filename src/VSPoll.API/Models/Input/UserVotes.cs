using System;
using System.ComponentModel.DataAnnotations;

namespace VSPoll.API.Models.Input
{
    public class UserVotes : Authenticated
    {
        [Required]
        public Guid Poll { get; set; }
    }
}
