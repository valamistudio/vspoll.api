using System.Threading.Tasks;
using VSPoll.API.Persistence.Entities;

namespace VSPoll.API.Persistence.Repositories;

public interface IUserRepository
{
    Task AddOrUpdateUserAsync(User user);
}
