using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VSPoll.API.Models.Input;
using VSPoll.API.Models.Output;
using VSPoll.API.Persistence.Repositories;
using Entity = VSPoll.API.Persistence.Entities;

namespace VSPoll.API.Services
{
    public class PollService : IPollService
    {
        private readonly IPollRepository pollRepository;
        public PollService(IPollRepository pollRepository)
            => this.pollRepository = pollRepository;

        public Task<bool> CheckIfPollExistsAsync(Guid id)
            => pollRepository.CheckIfPollExists(id);

        public async Task<Poll> GetPollAsync(Guid id)
        {
            var poll = await pollRepository.GetByIdAsync(id);
            return new Poll(poll);
        }

        public IEnumerable<Guid> GetVotes(Guid poll, int user)
            => pollRepository.GetVotes(poll, user);

        public async Task<Poll> InsertPollAsync(PollCreate pollCreate)
        {
            var poll = new Entity.Poll(pollCreate);
            await pollRepository.InsertPollAsync(poll);
            return new Poll(poll);
        }
    }
}
