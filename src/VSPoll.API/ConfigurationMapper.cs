using AutoMapper;
using Entity = VSPoll.API.Persistence.Entity;

namespace VSPoll.API
{
    public class ConfigurationMapper : Profile
    {
        public ConfigurationMapper()
            => CreateMap<Models.User, Entity.User>();
    }
}
