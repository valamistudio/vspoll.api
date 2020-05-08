using System.Threading.Tasks;
using VSPoll.API.Models;

namespace VSPoll.API.Services
{
    public interface IPollService
    {
        Task<Poll> GetPollAsync(int id);
    }
}
