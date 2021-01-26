using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VSPoll.API.Persistence.Contexts;
using VSPoll.API.Persistence.Entities;

namespace VSPoll.API.Persistence.Repositories
{
    public class PollRepository : IPollRepository
    {
        private readonly PollContext context;
        public PollRepository(PollContext context) => this.context = context;

        public Task<bool> CheckIfPollExists(Guid id)
            => context.Polls.AnyAsync(poll => poll.Id == id);

        //throws warning when omitting async/await, don't know why
        public async Task<Poll?> GetByIdAsync(Guid id)
            => await context.Polls.SingleOrDefaultAsync(poll => poll.Id == id);

        public IEnumerable<Guid> GetVotes(Guid poll, int user)
            => context.PollVotes.Where(vote => vote.Option.PollId == poll && vote.UserId == user)
                                .Select(vote => vote.OptionId);

        public async Task InsertPollAsync(Poll poll)
        {
            await context.Polls.AddAsync(poll);
            await context.SaveChangesAsync();
        }

        private IQueryable<PollOption> GetOptions(Guid poll)
            => context.PollOptions.Where(option => option.PollId == poll);

        public async Task UnvoteAsync(Guid poll, int user)
        {
            var options = GetOptions(poll).Select(option => option.Id).ToList();
            var votes = context.PollVotes.Where(vote => options.Contains(vote.OptionId) && vote.UserId == user).ToList();
            context.PollVotes.RemoveRange(votes);
            await context.SaveChangesAsync();
        }

        public int GetVotersCount(Guid id)
        {
            var options = GetOptions(id).Select(option => option.Id).ToList();
            return context.PollVotes.Where(vote => options.Contains(vote.OptionId))
                                    .Select(vote => vote.UserId)
                                    .Distinct().Count();
        }
    }
}
