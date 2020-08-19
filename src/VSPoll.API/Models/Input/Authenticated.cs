using System.ComponentModel.DataAnnotations;

namespace VSPoll.API.Models.Input
{
    public abstract class Authenticated
    {
        [Required]
        public Authentication User { get; set; } = null!;
    }
}
