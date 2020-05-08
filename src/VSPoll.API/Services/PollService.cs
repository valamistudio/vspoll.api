using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using VSPoll.API.Models;
using VSPoll.API.Persistence.Repository;

namespace VSPoll.API.Services
{
    public class PollService : IPollService
    {
        private readonly IPollRepository pollRepository;
        private readonly IMapper mapper;
        public PollService(IPollRepository pollRepository, IMapper mapper)
        {
            this.pollRepository = pollRepository;
            this.mapper = mapper;
        }

        public async Task<Poll> GetPollAsync(int id)
        {
            var poll = await pollRepository.GetByIdAsync(id);
            return new Poll
            {
                AllowAdd = poll.AllowAdd,
                EndDate = poll.EndDate,
                MultiVote = poll.MultiVote,
                Options = poll.Options.Select(option => new PollOption
                {
                    Description = option.Description,
                    Id = option.Id,
                    Percentage = option.Votes.Count / poll.Options.Sum(opt => opt.Votes.Count),
                    Voters = option.Votes.Select(vote => mapper.Map<User>(vote.User)),
                    Votes = option.Votes.Count,
                }),
                ShowVoters = poll.ShowVoters,
            };
        }
    }
}
