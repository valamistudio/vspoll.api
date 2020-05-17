using System;
using System.Threading.Tasks;
using VSPoll.API.Models;
using VSPoll.API.Persistence.Repository;
using Entity = VSPoll.API.Persistence.Entity;

namespace VSPoll.API.Services
{
    public class OptionService : IOptionService
    {
        private readonly IOptionRepository optionRepository;
        public OptionService(IOptionRepository optionRepository)
            => this.optionRepository = optionRepository;

        public Task<bool> CheckIfOptionExists(Guid id)
            => optionRepository.CheckIfOptionExists(id);

        public async Task<Page<User>> GetVotersAsync(VotersQuery query)
        {
            var voters = await optionRepository.GetVotersAsync(query);
            return voters.Map(voter => new User(voter));
        }

        public async Task<Poll> GetPollFromOptionAsync(Guid id)
        {
            var option = await optionRepository.GetByIdAsync(id);
            return new Poll(option.Poll);
        }

        public Task<bool> GetVoteStatusAsync(Guid option, int user)
            => optionRepository.GetVoteStatusAsync(option, user);

        public Task ClearVoteAsync(Guid poll, int user)
            => optionRepository.ClearVoteAsync(poll, user);

        public Task VoteAsync(Guid option, int user)
            => optionRepository.InsertVoteAsync(new Entity.PollVote
            {
                OptionId = option,
                UserId = user,
            });

        public Task UnvoteAsync(Guid option, int user)
            => optionRepository.DeleteVoteAsync(new Entity.PollVote
            {
                OptionId = option,
                UserId = user,
            });

        public async Task<PollOption> InsertOptionAsync(PollOptionCreate optionCreate)
        {
            var option = new Entity.PollOption(optionCreate);
            await optionRepository.InsertOptionAsync(option);
            return new PollOption(option);
        }

        public async Task<bool> CheckDuplicateAsync(PollOptionCreate optionCreate)
        {
            var option = new Entity.PollOption(optionCreate);
            return await optionRepository.CheckDuplicateAsync(option);
        }
    }
}
