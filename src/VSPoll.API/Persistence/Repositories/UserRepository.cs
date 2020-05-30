using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VSPoll.API.Persistence.Contexts;
using VSPoll.API.Persistence.Entities;

namespace VSPoll.API.Persistence.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly PollContext context;
        public UserRepository(PollContext context) => this.context = context;

        public async Task AddOrUpdateUserAsync(User user)
        {
            if (await context.Users.AnyAsync(u => u.Id == user.Id))
                context.Users.Update(user);
            else
                context.Users.Add(user);

            await context.SaveChangesAsync();
        }
    }
}
