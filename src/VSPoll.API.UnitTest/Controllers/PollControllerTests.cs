using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using VSPoll.API.Controllers;
using VSPoll.API.Extensions;
using VSPoll.API.Models.Input;
using VSPoll.API.Models.Output;
using VSPoll.API.Services;
using Xunit;

namespace VSPoll.API.UnitTest.Controllers
{
    public class PollControllerTests
    {
        [Fact]
        public async Task Get_UnknownId_ShouldReturnNotFound()
        {
            Mock<IPollService> service = new();
            service.Setup(x => x.CheckIfPollExistsAsync(It.IsAny<Guid>())).ReturnsAsync(false);

            PollController controller = new(service.Object, null);
            var ret = await controller.Get(Guid.NewGuid());
            ret.Result.Should().BeOfType<NotFoundObjectResult>();
        }

        [Fact]
        public async Task Get_KnownId_ShouldReturnOk()
        {
            Mock<IPollService> service = new();
            service.Setup(x => x.CheckIfPollExistsAsync(It.IsAny<Guid>())).ReturnsAsync(true);
            service.Setup(x => x.GetPollAsync(It.IsAny<Guid>())).ReturnsAsync(new Poll());

            PollController controller = new(service.Object, null);
            var ret = await controller.Get(Guid.NewGuid());
            ret.Result.Should().BeOfType<OkObjectResult>();
        }

        private static UserVotes NewValidUserVotes() => new UserVotes
        {
            Poll = Guid.NewGuid(),
            User = new Authentication(),
        };

        [Fact]
        public async Task GetVotes_MissingPayload_ShouldReturnBadRequest()
        {
            PollController controller = new(null, null);
            var ret = await controller.GetVotes(null);
            ret.Result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task GetVotes_MissingAuthentication_ShouldReturnBadRequest()
        {
            var input = NewValidUserVotes();
            input.User = null;

            PollController controller = new(null, null);
            var ret = await controller.GetVotes(input);
            ret.Result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task GetVotes_FailedAuthentication_ShouldReturnUnauthorized()
        {
            var input = NewValidUserVotes();

            Mock<IUserService> userService = new();
            userService.Setup(x => x.Authenticate(It.IsAny<Authentication>(), out It.Ref<string>.IsAny)).Returns(false);

            PollController controller = new(null, userService.Object);
            var ret = await controller.GetVotes(input);
            ret.Result.Should().BeOfType<UnauthorizedObjectResult>();
        }

        [Fact]
        public async Task GetVotes_UnknownPoll_ShouldReturnNotFound()
        {
            var input = NewValidUserVotes();

            Mock<IPollService> pollService = new();
            pollService.Setup(x => x.CheckIfPollExistsAsync(It.IsAny<Guid>())).ReturnsAsync(false);

            Mock<IUserService> userService = new();
            userService.Setup(x => x.Authenticate(It.IsAny<Authentication>(), out It.Ref<string>.IsAny)).Returns(true);

            PollController controller = new(pollService.Object, userService.Object);
            var ret = await controller.GetVotes(input);
            ret.Result.Should().BeOfType<NotFoundObjectResult>();
        }

        [Fact]
        public async Task GetVotes_ValidInput_ShouldReturnOk()
        {
            var input = NewValidUserVotes();

            Mock<IPollService> pollService = new();
            pollService.Setup(x => x.CheckIfPollExistsAsync(It.IsAny<Guid>())).ReturnsAsync(true);
            pollService.Setup(x => x.GetVotes(It.IsAny<Guid>(), It.IsAny<int>())).Returns(Enumerable.Empty<Guid>());

            Mock<IUserService> userService = new();
            userService.Setup(x => x.Authenticate(It.IsAny<Authentication>(), out It.Ref<string>.IsAny)).Returns(true);

            PollController controller = new(pollService.Object, userService.Object);
            var ret = await controller.GetVotes(input);
            ret.Result.Should().BeOfType<OkObjectResult>();
        }

        private static PollCreate NewValidPollCreate() => new PollCreate
        {
            Description = "foo",
            EndDate = DateTime.UtcNow.AddDays(7),
            Options = new[] { "a", "b", "c" },
        };

        [Fact]
        public async Task Post_MissingPayload_ShouldReturnBadRequest()
        {
            PollController controller = new(null, null);
            var ret = await controller.Post(null);
            ret.Result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task Post_ValidPoll_ShouldReturnOk()
        {
            var poll = NewValidPollCreate();

            Mock<IPollService> service = new();
            service.Setup(x => x.InsertPollAsync(It.IsAny<PollCreate>())).ReturnsAsync(new Poll());

            PollController controller = new(service.Object, null);
            var ret = await controller.Post(poll);
            ret.Result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task Post_MissingDescription_ShouldReturnBadRequest()
        {
            var poll = NewValidPollCreate();
            poll.Description = null;

            PollController controller = new(null, null);
            var ret = await controller.Post(poll);
            ret.Result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task Post_LongDescription_ShouldReturnBadRequest()
        {
            var poll = NewValidPollCreate();
            poll.Description = 'a'.Repeat(101).AppendAll();

            PollController controller = new(null, null);
            var ret = await controller.Post(poll);
            ret.Result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task Post_MissingEndDate_ShouldReturnBadRequest()
        {
            var poll = NewValidPollCreate();
            poll.EndDate = default;

            PollController controller = new(null, null);
            var ret = await controller.Post(poll);
            ret.Result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task Post_MissingOptions_ShouldReturnBadRequest()
        {
            var poll = NewValidPollCreate();
            poll.Options = null;

            PollController controller = new(null, null);
            var ret = await controller.Post(poll);
            ret.Result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task Post_FewOptions_ShouldReturnBadRequest()
        {
            var poll = NewValidPollCreate();
            poll.Options = new[] { "foo" };

            PollController controller = new(null, null);
            var ret = await controller.Post(poll);
            ret.Result.Should().BeOfType<BadRequestObjectResult>();
        }
    }
}
