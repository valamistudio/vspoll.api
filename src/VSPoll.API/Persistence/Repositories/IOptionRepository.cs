using System;
using System.Threading.Tasks;
using VSPoll.API.Persistence.Entities;

namespace VSPoll.API.Persistence.Repositories
{
    public interface IOptionRepository
    {
        Task<bool> CheckIfOptionExists(Guid id);
        Task<PollOption> GetByIdAsync(Guid id);
        Task<bool> GetVoteStatusAsync(Guid option, int user);
        Task<Models.Output.Page<User>> GetVotersAsync(Models.Input.VotersQuery query);
        Task ClearVoteAsync(Guid poll, int user);
        Task InsertVoteAsync(PollVote vote);
        Task DeleteVoteAsync(PollVote vote);
        Task InsertOptionAsync(PollOption option);
        Task<bool> CheckDuplicateAsync(PollOption option);
    }
}
