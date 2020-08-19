using System;
using System.ComponentModel.DataAnnotations;

namespace VSPoll.API.Models.Input
{
    public class Vote : Authenticated
    {
        [Required]
        public Guid Option { get; set; }
    }
}
