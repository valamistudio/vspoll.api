using System.Threading.Tasks;

namespace VSPoll.API.Persistence.Repository
{
    public interface IPollRepository
    {
        Task<Entity.Poll> GetByIdAsync(int id);
    }
}
