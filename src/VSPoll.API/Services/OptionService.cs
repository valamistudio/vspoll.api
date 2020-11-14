using System;
using System.Threading.Tasks;
using VSPoll.API.Models.Input;
using VSPoll.API.Models.Output;
using VSPoll.API.Persistence.Repositories;
using Entity = VSPoll.API.Persistence.Entities;

namespace VSPoll.API.Services
{
    public class OptionService : IOptionService
    {
        private readonly IOptionRepository optionRepository;
        public OptionService(IOptionRepository optionRepository)
            => this.optionRepository = optionRepository;

        public Task<bool> CheckIfOptionExistsAsync(Guid id)
            => optionRepository.CheckIfOptionExists(id);

        public async Task<Page<User>> GetVotersAsync(VotersQuery query)
        {
            var voters = await optionRepository.GetVotersAsync(query);
            return voters.Map(voter => new User(voter));
        }

        public async Task<Poll> GetPollFromOptionAsync(Guid id)
        {
            var option = await optionRepository.GetByIdAsync(id);
            return new(option.Poll);
        }

        public Task<bool> GetVoteStatusAsync(Guid option, int user)
            => optionRepository.GetVoteStatusAsync(option, user);

        public Task ClearVoteAsync(Guid poll, int user)
            => optionRepository.ClearVoteAsync(poll, user);

        public Task VoteAsync(Guid option, int user)
            => optionRepository.InsertVoteAsync(new()
            {
                OptionId = option,
                UserId = user,
            });

        public Task UnvoteAsync(Guid option, int user)
            => optionRepository.DeleteVoteAsync(new()
            {
                OptionId = option,
                UserId = user,
            });

        public async Task<PollOption> InsertOptionAsync(PollOptionCreate optionCreate)
        {
            Entity.PollOption option = new(optionCreate);
            await optionRepository.InsertOptionAsync(option);
            return new(option);
        }

        public async Task<bool> CheckDuplicateAsync(PollOptionCreate optionCreate)
        {
            Entity.PollOption option = new(optionCreate);
            return await optionRepository.CheckDuplicateAsync(option);
        }
    }
}
