using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using VSPoll.API.Controllers;
using VSPoll.API.Extensions;
using VSPoll.API.Models;
using VSPoll.API.Models.Input;
using VSPoll.API.Models.Output;
using VSPoll.API.Services;
using Xunit;

namespace VSPoll.API.UnitTest.Controllers;

[SuppressMessage("ReSharper", "AssignNullToNotNullAttribute")]
public class PollControllerTests
{
    private static RankedVote GetAuthenticatedRankedVote() => new()
    {
        User = new(),
    };

    [Fact]
    public async Task Get_UnknownId_ShouldReturnNotFound()
    {
        PollController controller = new(Mock.Of<IPollService>(), null, null);
        var ret = await controller.Get(Guid.NewGuid());
        ret.Result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task Get_KnownId_ShouldReturnOk()
    {
        Mock<IPollService> service = new();
        service.Setup(x => x.GetPollViewAsync(It.IsAny<Guid>(), It.IsAny<OptionSorting>())).ReturnsAsync(new PollView());

        PollController controller = new(service.Object, null, null);
        var ret = await controller.Get(Guid.NewGuid());
        ret.Result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task GetVotes_MissingAuthentication_ShouldReturnBadRequest()
    {
        PollController controller = new(null, null, null);
        var ret = await controller.GetVotes(Guid.NewGuid(), null);
        ret.Result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task GetVotes_FailedAuthentication_ShouldReturnUnauthorized()
    {
        Mock<IUserService> userService = new();
        userService.Setup(x => x.Authenticate(It.IsAny<Authentication>(), out It.Ref<string>.IsAny)).Returns(false);

        PollController controller = new(null, null, userService.Object);
        var ret = await controller.GetVotes(Guid.NewGuid(), new());
        ret.Result.Should().BeOfType<UnauthorizedObjectResult>();
    }

    [Fact]
    public async Task GetVotes_UnknownPoll_ShouldReturnNotFound()
    {
        Mock<IUserService> userService = new();
        userService.Setup(x => x.Authenticate(It.IsAny<Authentication>(), out It.Ref<string>.IsAny)).Returns(true);

        PollController controller = new(Mock.Of<IPollService>(), null, userService.Object);
        var ret = await controller.GetVotes(Guid.NewGuid(), new());
        ret.Result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task GetVotes_ValidInput_ShouldReturnOk()
    {
        Mock<IPollService> pollService = new();
        pollService.Setup(x => x.CheckIfPollExistsAsync(It.IsAny<Guid>())).ReturnsAsync(true);
        pollService.Setup(x => x.GetVotes(It.IsAny<Guid>(), It.IsAny<int>())).Returns(Enumerable.Empty<Guid>());

        Mock<IUserService> userService = new();
        userService.Setup(x => x.Authenticate(It.IsAny<Authentication>(), out It.Ref<string>.IsAny)).Returns(true);

        PollController controller = new(pollService.Object, null, userService.Object);
        var ret = await controller.GetVotes(Guid.NewGuid(), new());
        ret.Result.Should().BeOfType<OkObjectResult>();
    }

    private static PollCreate NewValidPollCreate() => new()
    {
        Description = "foo",
        EndDate = DateTime.UtcNow.AddDays(7),
        Options = new[] { "a", "b", "c" },
    };

    [Fact]
    public async Task Post_MissingPayload_ShouldReturnBadRequest()
    {
        PollController controller = new(null, null, null);
        var ret = await controller.Post(null);
        ret.Result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task Post_ValidPoll_ShouldReturnOk()
    {
        var poll = NewValidPollCreate();

        Mock<IPollService> service = new();
        service.Setup(x => x.InsertPollAsync(It.IsAny<PollCreate>())).ReturnsAsync(new Poll());

        PollController controller = new(service.Object, null, null);
        var ret = await controller.Post(poll);
        ret.Result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task Post_MissingDescription_ShouldReturnBadRequest()
    {
        var poll = NewValidPollCreate();
        poll.Description = null;

        PollController controller = new(null, null, null);
        var ret = await controller.Post(poll);
        ret.Result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task Post_LongDescription_ShouldReturnBadRequest()
    {
        var poll = NewValidPollCreate();
        poll.Description = 'a'.Repeat(101).AppendAll();

        PollController controller = new(null, null, null);
        var ret = await controller.Post(poll);
        ret.Result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task Post_MissingEndDate_ShouldReturnBadRequest()
    {
        var poll = NewValidPollCreate();
        poll.EndDate = default;

        PollController controller = new(null, null, null);
        var ret = await controller.Post(poll);
        ret.Result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task Post_MissingOptions_ShouldReturnBadRequest()
    {
        var poll = NewValidPollCreate();
        poll.Options = null;

        PollController controller = new(null, null, null);
        var ret = await controller.Post(poll);
        ret.Result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task Post_FewOptions_ShouldReturnBadRequest()
    {
        var poll = NewValidPollCreate();
        poll.Options = new[] { "foo" };

        PollController controller = new(null, null, null);
        var ret = await controller.Post(poll);
        ret.Result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task Post_RankedAllowAdd_ShouldReturnBadRequest()
    {
        var poll = NewValidPollCreate();
        poll.VotingSystem = VotingSystem.Ranked;
        poll.AllowAdd = true;

        PollController controller = new(null, null, null);
        var ret = await controller.Post(poll);
        ret.Result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task Post_RankedShowVoters_ShouldReturnBadRequest()
    {
        var poll = NewValidPollCreate();
        poll.VotingSystem = VotingSystem.Ranked;
        poll.ShowVoters = true;

        PollController controller = new(null, null, null);
        var ret = await controller.Post(poll);
        ret.Result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task Post_ValidRankedPoll_ShouldReturnOk()
    {
        var poll = NewValidPollCreate();
        poll.VotingSystem = VotingSystem.Ranked;

        Mock<IPollService> service = new();
        service.Setup(x => x.InsertPollAsync(It.IsAny<PollCreate>())).ReturnsAsync(new Poll());

        PollController controller = new(service.Object, null, null);
        var ret = await controller.Post(poll);
        ret.Result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task Vote_MissingAuthentication_ShouldReturnBadRequest()
    {
        PollController controller = new(null, null, null);
        var ret = await controller.Vote(Guid.NewGuid(), null);
        ret.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task Vote_FailedAuthentication_ShouldUnauthorized()
    {
        Mock<IUserService> userService = new();
        userService.Setup(x => x.Authenticate(It.IsAny<Authentication>(), out It.Ref<string>.IsAny)).Returns(false);

        PollController controller = new(null, null, userService.Object);
        var ret = await controller.Vote(Guid.NewGuid(), GetAuthenticatedRankedVote());
        ret.Should().BeOfType<UnauthorizedObjectResult>();
    }

    [Fact]
    public async Task Vote_UnknownPoll_ShouldReturnNotFound()
    {
        Mock<IUserService> userService = new();
        userService.Setup(x => x.Authenticate(It.IsAny<Authentication>(), out It.Ref<string>.IsAny)).Returns(true);

        PollController controller = new(Mock.Of<IPollService>(), null, userService.Object);
        var ret = await controller.Vote(Guid.NewGuid(), GetAuthenticatedRankedVote());
        ret.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task Vote_PollExpired_ShouldReturnConflict()
    {
        Mock<IPollService> pollService = new();
        pollService.Setup(x => x.GetPollAsync(It.IsAny<Guid>())).ReturnsAsync(new Poll());

        Mock<IUserService> userService = new();
        userService.Setup(x => x.Authenticate(It.IsAny<Authentication>(), out It.Ref<string>.IsAny)).Returns(true);

        PollController controller = new(pollService.Object, null, userService.Object);
        var ret = await controller.Vote(Guid.NewGuid(), GetAuthenticatedRankedVote());
        ret.Should().BeOfType<ConflictObjectResult>();
    }

    [Fact]
    public async Task Vote_NullOptions_ShouldReturnBadRequest()
    {
        Mock<IPollService> pollService = new();
        pollService.Setup(x => x.GetPollAsync(It.IsAny<Guid>())).ReturnsAsync(new Poll
        {
            EndDate = DateTime.UtcNow.AddDays(7),
        });

        Mock<IUserService> userService = new();
        userService.Setup(x => x.Authenticate(It.IsAny<Authentication>(), out It.Ref<string>.IsAny)).Returns(true);

        PollController controller = new(pollService.Object, null, userService.Object);
        var ret = await controller.Vote(Guid.NewGuid(), GetAuthenticatedRankedVote());
        ret.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task Vote_EmptyOptions_ShouldReturnBadRequest()
    {
        Mock<IPollService> pollService = new();
        pollService.Setup(x => x.GetPollAsync(It.IsAny<Guid>())).ReturnsAsync(new Poll
        {
            EndDate = DateTime.UtcNow.AddDays(7),
        });

        Mock<IUserService> userService = new();
        userService.Setup(x => x.Authenticate(It.IsAny<Authentication>(), out It.Ref<string>.IsAny)).Returns(true);

        PollController controller = new(pollService.Object, null, userService.Object);
        var ret = await controller.Vote(Guid.NewGuid(), GetAuthenticatedRankedVote() with
        {
            Options = Enumerable.Empty<Guid>(),
        });
        ret.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task Vote_DuplicateRank_ShouldReturnBadRequest()
    {
        Mock<IPollService> pollService = new();
        pollService.Setup(x => x.GetPollAsync(It.IsAny<Guid>())).ReturnsAsync(new Poll
        {
            EndDate = DateTime.UtcNow.AddDays(7),
            VotingSystem = VotingSystem.Ranked,
        });

        Mock<IUserService> userService = new();
        userService.Setup(x => x.Authenticate(It.IsAny<Authentication>(), out It.Ref<string>.IsAny)).Returns(true);

        PollController controller = new(pollService.Object, null, userService.Object);
        var guid = Guid.NewGuid();
        var ret = await controller.Vote(Guid.NewGuid(), GetAuthenticatedRankedVote() with
        {
            Options = new[] { guid, guid },
        });
        ret.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task Vote_MultipleOptionsOnSingleOptionPoll_ShouldReturnBadRequest()
    {
        Mock<IPollService> pollService = new();
        pollService.Setup(x => x.GetPollAsync(It.IsAny<Guid>())).ReturnsAsync(new Poll
        {
            EndDate = DateTime.UtcNow.AddDays(7),
        });

        Mock<IUserService> userService = new();
        userService.Setup(x => x.Authenticate(It.IsAny<Authentication>(), out It.Ref<string>.IsAny)).Returns(true);

        PollController controller = new(pollService.Object, null, userService.Object);
        var ret = await controller.Vote(Guid.NewGuid(), GetAuthenticatedRankedVote() with
        {
            Options = new[] { Guid.NewGuid(), Guid.NewGuid() },
        });
        ret.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task Vote_NullOption_ShouldReturnNotFound()
    {
        Mock<IPollService> pollService = new();
        pollService.Setup(x => x.GetPollAsync(It.IsAny<Guid>())).ReturnsAsync(new Poll
        {
            EndDate = DateTime.UtcNow.AddDays(7),
        });

        Mock<IOptionService> optionService = new();
        optionService.Setup(x => x.GetOptionAsync(It.IsAny<Guid>())).ReturnsAsync((PollOption)null);

        Mock<IUserService> userService = new();
        userService.Setup(x => x.Authenticate(It.IsAny<Authentication>(), out It.Ref<string>.IsAny)).Returns(true);

        PollController controller = new(pollService.Object, optionService.Object, userService.Object);
        var ret = await controller.Vote(Guid.NewGuid(), GetAuthenticatedRankedVote() with
        {
            Options = new[] { Guid.NewGuid() },
        });
        ret.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task Vote_OptionFromAnotherPoll_ShouldReturnNotFound()
    {
        Mock<IPollService> pollService = new();
        pollService.Setup(x => x.GetPollAsync(It.IsAny<Guid>())).ReturnsAsync(new Poll
        {
            EndDate = DateTime.UtcNow.AddDays(7),
            Id = Guid.NewGuid(),
        });

        Mock<IOptionService> optionService = new();
        optionService.Setup(x => x.GetOptionAsync(It.IsAny<Guid>())).ReturnsAsync(new PollOption
        {
            Poll = Guid.NewGuid(),
        });

        Mock<IUserService> userService = new();
        userService.Setup(x => x.Authenticate(It.IsAny<Authentication>(), out It.Ref<string>.IsAny)).Returns(true);

        PollController controller = new(pollService.Object, optionService.Object, userService.Object);
        var ret = await controller.Vote(Guid.NewGuid(), GetAuthenticatedRankedVote() with
        {
            Options = new[] { Guid.NewGuid() },
        });
        ret.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task Vote_ValidInput_ShouldReturnOk()
    {
        var pollId = Guid.NewGuid();
        Mock<IPollService> pollService = new();
        pollService.Setup(x => x.GetPollAsync(It.IsAny<Guid>())).ReturnsAsync(new Poll
        {
            EndDate = DateTime.UtcNow.AddDays(7),
        });

        Mock<IOptionService> optionService = new();
        optionService.Setup(x => x.GetOptionAsync(It.IsAny<Guid>())).ReturnsAsync(new PollOption
        {
            Poll = pollId,
        });

        Mock<IUserService> userService = new();
        userService.Setup(x => x.Authenticate(It.IsAny<Authentication>(), out It.Ref<string>.IsAny)).Returns(true);

        PollController controller = new(pollService.Object, optionService.Object, userService.Object);
        var ret = await controller.Vote(pollId, GetAuthenticatedRankedVote() with
        {
            Options = new[] { Guid.NewGuid() },
        });
        ret.Should().BeOfType<OkResult>();
    }

    [Fact]
    public async Task Unvote_MissingAuthentication_ShouldReturnBadRequest()
    {
        PollController controller = new(null, null, null);
        var ret = await controller.Unvote(Guid.NewGuid(), null);
        ret.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task Unvote_FailedAuthentication_ShouldUnauthorized()
    {
        Mock<IUserService> userService = new();
        userService.Setup(x => x.Authenticate(It.IsAny<Authentication>(), out It.Ref<string>.IsAny)).Returns(false);

        PollController controller = new(null, null, userService.Object);
        var ret = await controller.Unvote(Guid.NewGuid(), new());
        ret.Should().BeOfType<UnauthorizedObjectResult>();
    }

    [Fact]
    public async Task Unvote_UnknownPoll_ShouldReturnNotFound()
    {
        Mock<IUserService> userService = new();
        userService.Setup(x => x.Authenticate(It.IsAny<Authentication>(), out It.Ref<string>.IsAny)).Returns(true);

        PollController controller = new(Mock.Of<IPollService>(), null, userService.Object);
        var ret = await controller.Unvote(Guid.NewGuid(), new());
        ret.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task Unvote_PollExpired_ShouldReturnConflict()
    {
        Mock<IPollService> pollService = new();
        pollService.Setup(x => x.GetPollAsync(It.IsAny<Guid>())).ReturnsAsync(new Poll());

        Mock<IUserService> userService = new();
        userService.Setup(x => x.Authenticate(It.IsAny<Authentication>(), out It.Ref<string>.IsAny)).Returns(true);

        PollController controller = new(pollService.Object, null, userService.Object);
        var ret = await controller.Unvote(Guid.NewGuid(), new());
        ret.Should().BeOfType<ConflictObjectResult>();
    }

    [Fact]
    public async Task Unvote_ValidInput_ShouldReturnOk()
    {
        Mock<IPollService> pollService = new();
        pollService.Setup(x => x.GetPollAsync(It.IsAny<Guid>())).ReturnsAsync(new Poll
        {
            EndDate = DateTime.UtcNow.AddDays(7),
        });

        Mock<IUserService> userService = new();
        userService.Setup(x => x.Authenticate(It.IsAny<Authentication>(), out It.Ref<string>.IsAny)).Returns(true);

        PollController controller = new(pollService.Object, null, userService.Object);
        var ret = await controller.Unvote(Guid.NewGuid(), new());
        ret.Should().BeOfType<OkResult>();
    }
}
