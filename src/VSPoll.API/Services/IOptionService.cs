using System;
using System.Threading.Tasks;
using VSPoll.API.Models;

namespace VSPoll.API.Services
{
    public interface IOptionService
    {
        Task<bool> CheckIfOptionExistsAsync(Guid id);
        Task<Page<User>> GetVotersAsync(VotersQuery query);
        Task<Poll> GetPollFromOptionAsync(Guid id);
        Task ClearVoteAsync(Guid poll, int user);
        Task VoteAsync(Guid option, int user);
        Task UnvoteAsync(Guid option, int user);
        Task<PollOption> InsertOptionAsync(PollOptionCreate optionCreate);
        Task<bool> CheckDuplicateAsync(PollOptionCreate optionCreate);
        Task<bool> GetVoteStatusAsync(Guid option, int user);
    }
}
