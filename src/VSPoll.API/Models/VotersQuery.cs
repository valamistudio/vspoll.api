using System;
using System.ComponentModel.DataAnnotations;

namespace VSPoll.API.Models
{
    public class VotersQuery : Paged
    {
        [Required]
        public Guid Option { get; set; }
    }
}
