using Microsoft.Extensions.DependencyInjection;
using VSPoll.API.Persistence.Repository;
using VSPoll.API.Services;

namespace VSPoll.API.Extensions
{
    public static class ServiceCollection
    {
        public static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            services.AddTransient<IPollRepository, PollRepository>();
            return services;
        }

        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            services.AddTransient<IPollService, PollService>();
            return services;
        }
    }
}
