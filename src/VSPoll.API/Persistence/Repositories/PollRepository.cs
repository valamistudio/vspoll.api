﻿using System;
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

        public Task<Poll> GetByIdAsync(Guid id)
            => context.Polls.SingleAsync(poll => poll.Id == id);

        public IEnumerable<Guid> GetVotes(Guid poll, int user)
            => context.PollVotes.Where(vote => vote.Option.PollId == poll && vote.UserId == user)
                                .Select(vote => vote.OptionId);

        public async Task InsertPollAsync(Poll poll)
        {
            await context.Polls.AddAsync(poll);
            await context.SaveChangesAsync();
        }
    }
}
