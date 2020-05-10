using System;
using System.Threading.Tasks;
using VSPoll.API.Models;
using VSPoll.API.Persistence.Repository;
using Entity = VSPoll.API.Persistence.Entity;

namespace VSPoll.API.Services
{
    public class PollService : IPollService
    {
        private readonly IPollRepository pollRepository;
        public PollService(IPollRepository pollRepository)
            => this.pollRepository = pollRepository;

        public Task<bool> CheckIfPollExists(Guid id)
            => pollRepository.CheckIfPollExists(id);

        public async Task<Poll> GetPollAsync(Guid id)
        {
            var poll = await pollRepository.GetByIdAsync(id);
            return new Poll(poll);
        }

        public async Task<Poll> InsertPollAsync(PollCreate pollCreate)
        {
            var poll = new Entity.Poll(pollCreate);
            await pollRepository.InsertPollAsync(poll);
            return new Poll(poll);
        }
    }
}
