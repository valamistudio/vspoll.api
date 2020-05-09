using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VSPoll.API.Persistence.Context;
using VSPoll.API.Persistence.Entity;

namespace VSPoll.API.Persistence.Repository
{
    public class PollRepository : IPollRepository
    {
        private readonly PollContext context;
        public PollRepository(PollContext context) => this.context = context;

        public Task<Poll> GetByIdAsync(Guid id)
            => context.Polls.SingleAsync(poll => poll.Id == id);

        public async Task InsertPollAsync(Poll poll)
        {
            await context.Polls.AddAsync(poll);
            await context.SaveChangesAsync();
        }
    }
}
