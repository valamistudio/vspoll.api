using Microsoft.EntityFrameworkCore;
using VSPoll.API.Persistence.Entity;

namespace VSPoll.API.Persistence.Context
{
    public class PollContext : DbContext
    {
        public DbSet<Poll> Polls { get; set; } = null!;
        public DbSet<PollOption> PollOptions { get; set; } = null!;
        public DbSet<PollVote> PollVotes { get; set; } = null!;
        public DbSet<User> Users { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
            => modelBuilder.Entity<PollVote>().HasKey(vote => new { vote.OptionId, vote.UserId });

        public PollContext(DbContextOptions<PollContext> contextOptions) : base(contextOptions) { }
    }
}
