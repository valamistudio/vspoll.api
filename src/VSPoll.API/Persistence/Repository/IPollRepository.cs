using System;
using System.Threading.Tasks;
using VSPoll.API.Persistence.Entity;

namespace VSPoll.API.Persistence.Repository
{
    public interface IPollRepository
    {
        Task<Poll> GetByIdAsync(Guid id);
        Task InsertPollAsync(Poll poll);
    }
}
