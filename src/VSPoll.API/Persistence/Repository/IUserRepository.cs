using System.Threading.Tasks;
using VSPoll.API.Persistence.Entity;

namespace VSPoll.API.Persistence.Repository
{
    public interface IUserRepository
    {
        Task AddOrUpdateUserAsync(User user);
    }
}
