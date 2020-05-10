using System;
using System.Threading.Tasks;
using VSPoll.API.Models;

namespace VSPoll.API.Services
{
    public interface IPollService
    {
        Task<bool> CheckIfPollExists(Guid id);
        Task<Poll> GetPollAsync(Guid id);
        Task<Poll> InsertPollAsync(PollCreate pollCreate);
    }
}
