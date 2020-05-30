using System;
using System.Threading.Tasks;
using VSPoll.API.Persistence.Entities;

namespace VSPoll.API.Persistence.Repositories
{
    public interface IPollRepository
    {
        Task<bool> CheckIfPollExists(Guid id);
        Task<Poll> GetByIdAsync(Guid id);
        Task InsertPollAsync(Poll poll);
    }
}
