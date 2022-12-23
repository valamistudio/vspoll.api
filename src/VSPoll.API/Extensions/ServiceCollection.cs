using Microsoft.Extensions.DependencyInjection;
using VSPoll.API.Persistence.Repositories;
using VSPoll.API.Services;

namespace VSPoll.API.Extensions;

public static class ServiceCollection
{
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddTransient<IPollRepository, PollRepository>();
        services.AddTransient<IOptionRepository, OptionRepository>();
        services.AddTransient<IUserRepository, UserRepository>();
        return services;
    }

    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddTransient<IPollService, PollService>();
        services.AddTransient<IOptionService, OptionService>();
        services.AddTransient<IUserService, UserService>();
        return services;
    }
}
