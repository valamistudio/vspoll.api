using System;
using FluentAssertions;
using VSPoll.API.Models.Output;
using Xunit;
using Entity = VSPoll.API.Persistence.Entities;

namespace VSPoll.API.UnitTest.Models;

public class Mapping
{
    [Fact]
    public void Poll_Mapping()
    {
        Entity.Poll entity = new()
        {
            Description = "Description",
            EndDate = DateTime.UtcNow,
            Id = Guid.NewGuid(),
        };
        var model = Poll.Of(entity);
        model.AllowAdd.Should().Be(entity.AllowAdd);
        model.Description.Should().Be(entity.Description);
        model.EndDate.Should().Be(entity.EndDate);
        model.Id.Should().Be(entity.Id);
        model.VotingSystem.Should().Be(entity.VotingSystem);
        model.ShowVoters.Should().Be(entity.ShowVoters);
    }

    [Fact]
    public void PollOption_Mapping()
    {
        Entity.PollOption entity = new()
        {
            Description = "Description",
            Id = Guid.NewGuid(),
        };
        var model = PollOption.Of(entity);
        model.Description.Should().Be(entity.Description);
        model.Id.Should().Be(entity.Id);
    }

    [Fact]
    public void User_Mapping()
    {
        Entity.User entity = new()
        {
            FirstName = "First",
            Id = new Random().Next(),
            LastName = "Last",
            PhotoUrl = "Url",
            Username = "Username",
        };
        User model = new(entity);
        model.FirstName.Should().Be(entity.FirstName);
        model.LastName.Should().Be(entity.LastName);
        model.PhotoUrl.Should().Be(entity.PhotoUrl);
        model.Username.Should().Be(entity.Username);
    }
}
