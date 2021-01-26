using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VSPoll.API.Models.Input;
using VSPoll.API.Models.Output;

namespace VSPoll.API.Services
{
    public interface IPollService
    {
        Task<bool> CheckIfPollExistsAsync(Guid id);
        Task<Poll?> GetPollAsync(Guid id);
        Task<PollView?> GetPollViewAsync(Guid id, OptionSorting sort = OptionSorting.Votes);
        IEnumerable<Guid> GetVotes(Guid poll, int user);
        Task<Poll> InsertPollAsync(PollCreate pollCreate);
        Task UnvoteAsync(Guid poll, int user);
        Task VoteAsync(Poll poll, int user, IEnumerable<Guid> options);
    }
}
