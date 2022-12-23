using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using VSPoll.API.Models.Input;

namespace VSPoll.API.Services;

public interface IUserService
{
    bool Authenticate(Authentication authentication, [NotNullWhen(false)] out string? error);
    Task AddOrUpdateUserAsync(Authentication authentication);
}
