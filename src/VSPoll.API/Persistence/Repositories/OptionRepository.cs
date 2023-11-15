using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VSPoll.API.Extensions;
using VSPoll.API.Persistence.Contexts;
using VSPoll.API.Persistence.Entities;

namespace VSPoll.API.Persistence.Repositories;

public class OptionRepository(PollContext context) : IOptionRepository
{
    public Task<bool> CheckIfOptionExists(Guid id)
        => context.PollOptions.AnyAsync(option => option.Id == id);

    //throws warning when omitting async/await, don't know why
    public async Task<PollOption?> GetByIdAsync(Guid id)
        => await context.PollOptions.SingleOrDefaultAsync(option => option.Id == id);

    public Task<bool> GetVoteStatusAsync(Guid option, int user)
        => context.PollVotes.AnyAsync(vote => vote.OptionId == option && vote.UserId == user);

    public async Task<Models.Output.Page<User>> GetVotersAsync(Models.Input.VotersQuery query)
    {
        var items = context.PollVotes.AsQueryable();
        items = items.Where(item => item.OptionId == query.Option);
        var totalItems = await items.CountAsync();
        items = items.OrderBy(item => item.ReferenceDate)
            .Page(query)
            .Take(query.PageSize);
        return new
        (
            query.PageSize,
            query.Page,
            totalItems,
            await items.Select(item => item.User).ToListAsync()
        );
    }

    public async Task ClearVoteAsync(Guid poll, int user)
    {
        context.PollVotes.RemoveRange(context.PollVotes.Where(vote => vote.Option.PollId == poll && vote.UserId == user));
        await context.SaveChangesAsync();
    }

    public async Task InsertVotesAsync(params PollVote[] votes)
    {
        context.PollVotes.AddRange(votes);
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
