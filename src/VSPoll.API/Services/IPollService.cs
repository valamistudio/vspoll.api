using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VSPoll.API.Models;

namespace VSPoll.API.Services
{
    public interface IPollService
    {
        Task<bool> CheckIfPollExistsAsync(Guid id);
        Task<Poll> GetPollAsync(Guid id);
        IEnumerable<Guid> GetVotes(Guid poll, int user);
        Task<Poll> InsertPollAsync(PollCreate pollCreate);
    }
}
