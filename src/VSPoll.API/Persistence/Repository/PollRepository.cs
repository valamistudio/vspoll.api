using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VSPoll.API.Persistence.Context;

namespace VSPoll.API.Persistence.Repository
{
    public class PollRepository : IPollRepository
    {
        private readonly PollContext context;
        public PollRepository(PollContext context) => this.context = context;

        public async Task<Entity.Poll> GetByIdAsync(int id)
            => await context.Polls.SingleAsync(poll => poll.Id == id);
    }
}
