using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VSPoll.API.Models;
using VSPoll.API.Models.Input;
using VSPoll.API.Models.Output;
using VSPoll.API.Persistence.Repositories;
using VSPoll.API.Utils;
using Entity = VSPoll.API.Persistence.Entities;

namespace VSPoll.API.Services
{
    public class PollService : IPollService
    {
        private readonly IPollRepository pollRepository;
        private readonly IOptionRepository optionRepository;
        public PollService(IPollRepository pollRepository, IOptionRepository optionRepository)
        {
            this.pollRepository = pollRepository;
            this.optionRepository = optionRepository;
        }

        public Task<bool> CheckIfPollExistsAsync(Guid id)
            => pollRepository.CheckIfPollExists(id);

        public async Task<Poll?> GetPollAsync(Guid id)
        {
            var entity = await pollRepository.GetByIdAsync(id);
            return Poll.Of(entity);
        }

        public async Task<PollView?> GetPollViewAsync(Guid id, OptionSorting sort = OptionSorting.Votes)
        {
            var poll = await GetPollAsync(id);
            var view = PollView.Of(poll);
            if (view is not null)
            {
                view.Options = sort switch
                {
                    OptionSorting.Name => view.Options.OrderBy(option => option.Description),
                    OptionSorting.Votes => view.Options.OrderBy(option => option.Votes),
                    _ => throw new NotImplementedException(),
                };
                view.Options = view.Options.ToList();
                MathUtils.NormalizePercentages(view.Options);
                view.Voters = pollRepository.GetVotersCount(id);
            }
            return view;
        }

        public IEnumerable<Guid> GetVotes(Guid poll, int user)
            => pollRepository.GetVotes(poll, user);

        public async Task<Poll> InsertPollAsync(PollCreate pollCreate)
        {
            Entity.Poll poll = new(pollCreate);
            await pollRepository.InsertPollAsync(poll);
            return Poll.Of(poll);
        }

        public Task UnvoteAsync(Guid poll, int user)
            => pollRepository.UnvoteAsync(poll, user);

        public async Task VoteAsync(Poll poll, int user, IEnumerable<Guid> options)
        {
            if (poll.VotingSystem == VotingSystem.Ranked)
            {
                var optionsCount = poll.Options.Count();
                await optionRepository.InsertVotesAsync(options.Select(option => new Entity.PollVote
                {
                    OptionId = option,
                    UserId = user,
                    Weight = optionsCount--,
                }).ToArray());
            }
            else
                await optionRepository.InsertVotesAsync(options.Select(option => new Entity.PollVote
                {
                    OptionId = option,
                    UserId = user,
                }).ToArray());
        }
    }
}
