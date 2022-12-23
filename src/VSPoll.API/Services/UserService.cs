using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using VSPoll.API.Models.Input;
using VSPoll.API.Persistence.Repositories;

namespace VSPoll.API.Services;

public class UserService : IUserService
{
    private readonly IUserRepository userRepository;
    private readonly IConfiguration configuration;
    public UserService(IUserRepository userRepository, IConfiguration configuration)
    {
        this.userRepository = userRepository;
        this.configuration = configuration;
    }

    public bool Authenticate(Authentication authentication, [NotNullWhen(false)] out string? error)
    {
        var dataCheck = $"auth_date={authentication.AuthDate}\nfirst_name={authentication.FirstName}\nid={authentication.Id}";
        if (authentication.LastName is not null)
            dataCheck += $"\nlast_name={authentication.LastName}";

        if (authentication.PhotoUrl is not null)
            dataCheck += $"\nphoto_url={authentication.PhotoUrl}";

        if (authentication.Username is not null)
            dataCheck += $"\nusername={authentication.Username}";

        var token = configuration.GetSection("Secrets").GetValue<string>("BotToken");
        var secretKey = SHA256.HashData(Encoding.UTF8.GetBytes(token));
        using HMACSHA256 hmac = new(secretKey);
        var testHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(dataCheck));
        if (string.Join("", testHash.Select(b => b.ToString("x2"))) != authentication.Hash)
        {
            error = "Data is not from Telegram";
            return false;
        }
        if ((DateTime.UtcNow - DateTimeOffset.FromUnixTimeSeconds(authentication.AuthDate).LocalDateTime).Days > 6)
        {
            error = "Data is outdated";
            return false;
        }
        error = null;
        return true;
    }

    public Task AddOrUpdateUserAsync(Authentication authentication)
        => userRepository.AddOrUpdateUserAsync(new(authentication));
}
