using System;
using FluentAssertions;
using VSPoll.API.Models.Output;
using Xunit;
using Entity = VSPoll.API.Persistence.Entities;

namespace VSPoll.API.UnitTest.Models
{
    public class Mapping
    {
        [Fact]
        public void Poll_Mapping()
        {
            var entity = new Entity.Poll
            {
                Description = "Description",
                EndDate = DateTime.UtcNow,
                Id = Guid.NewGuid(),
            };
            var model = new Poll(entity);
            model.AllowAdd.Should().Be(entity.AllowAdd);
            model.Description.Should().Be(entity.Description);
            model.EndDate.Should().Be(entity.EndDate);
            model.Id.Should().Be(entity.Id);
            model.MultiVote.Should().Be(entity.MultiVote);
            model.ShowVoters.Should().Be(entity.ShowVoters);
        }

        [Fact]
        public void PollOption_Mapping()
        {
            var entity = new Entity.PollOption
            {
                Description = "Description",
                Id = Guid.NewGuid(),
            };
            var model = new PollOption(entity);
            model.Description.Should().Be(entity.Description);
            model.Id.Should().Be(entity.Id);
        }

        [Fact]
        public void User_Mapping()
        {
            var entity = new Entity.User
            {
                FirstName = "First",
                Id = new Random().Next(),
                LastName = "Last",
                PhotoUrl = "Url",
                Username = "Username",
            };
            var model = new User(entity);
            model.FirstName.Should().Be(entity.FirstName);
            model.LastName.Should().Be(entity.LastName);
            model.PhotoUrl.Should().Be(entity.PhotoUrl);
            model.Username.Should().Be(entity.Username);
        }
    }
}
