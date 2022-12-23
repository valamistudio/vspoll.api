using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VSPoll.API.Persistence.Entities;

namespace VSPoll.API.Persistence.Repositories;

public interface IPollRepository
{
    Task<bool> CheckIfPollExists(Guid id);
    Task<Poll?> GetByIdAsync(Guid id);
    int GetVotersCount(Guid id);
    IEnumerable<Guid> GetVotes(Guid poll, int user);
    Task InsertPollAsync(Poll poll);
    Task UnvoteAsync(Guid poll, int user);
}
