using System.ComponentModel.DataAnnotations;

namespace VSPoll.API.Models.Input;

public class Authentication
{
    [Required]
    public int Id { get; set; }

    [Required]
    public string FirstName { get; set; } = null!;

    public string? LastName { get; set; }

    public string? Username { get; set; }

    public string? PhotoUrl { get; set; }

    [Required]
    public long AuthDate { get; set; }

    [Required]
    public string Hash { get; set; } = null!;
}
