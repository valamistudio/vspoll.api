using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using VSPoll.API.Persistence.Context;
using VSPoll.API.Persistence.Entity;

namespace VSPoll.API.Persistence.Repository
{
    public class OptionRepository : IOptionRepository
    {
        private readonly PollContext context;
        public OptionRepository(PollContext context) => this.context = context;

        public Task<bool> CheckIfOptionExists(Guid id)
            => context.PollOptions.AnyAsync(option => option.Id == id);

        public Task<PollOption> GetByIdAsync(Guid id)
            => context.PollOptions.SingleAsync(option => option.Id == id);

        public Task<bool> GetVoteStatusAsync(Guid option, int user)
            => context.PollVotes.AnyAsync(vote => vote.OptionId == option && vote.UserId == user);

        public async Task<Models.Page<User>> GetVotersAsync(Models.VotersQuery query)
        {
            var items = context.PollVotes.AsQueryable();
            items = items.Where(item => item.OptionId == query.Option);
            var totalItems = await items.CountAsync();
            items = items.OrderBy(item => item.ReferenceDate)
                         .Skip((query.Page - 1) * query.PageSize)
                         .Take(query.PageSize);
            return new Models.Page<User>
            {
                Items = await items.Select(item => item.User).ToListAsync(),
                Number = query.Page,
                Size = query.PageSize,
                TotalItems = totalItems,
            };
        }

        public async Task ClearVoteAsync(Guid poll, int user)
        {
            context.PollVotes.RemoveRange(context.PollVotes.Where(vote => vote.Option.PollId == poll && vote.UserId == user));
            await context.SaveChangesAsync();
        }

        public async Task InsertVoteAsync(PollVote vote)
        {
            context.PollVotes.Add(vote);
            await context.SaveChangesAsync();
        }

        public async Task DeleteVoteAsync(PollVote vote)
        {
            context.PollVotes.Remove(vote);
            await context.SaveChangesAsync();
        }

        public async Task InsertOptionAsync(PollOption option)
        {
            //var poll = await context.Polls.SingleAsync(poll => poll.Id == option.PollId);
            //poll.Options.Add(option);
            context.PollOptions.Add(option);
            await context.SaveChangesAsync();
        }

        public Task<bool> CheckDuplicateAsync(PollOption option)
        {
            //var poll = await context.Polls.SingleAsync(poll => poll.Id == option.PollId);
            //return poll.Options.Any(opt => opt.Description == option.Description);
            return context.PollOptions.AnyAsync(opt => opt.PollId == option.PollId && opt.Description == option.Description);
        }
    }
}
