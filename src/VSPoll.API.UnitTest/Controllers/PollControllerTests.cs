using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using VSPoll.API.Controllers;
using VSPoll.API.Extensions;
using VSPoll.API.Models;
using VSPoll.API.Services;
using Xunit;

namespace VSPoll.API.UnitTest.Controllers
{
    public class PollControllerTests
    {
        [Fact]
        public async Task Get_UnknownId_ShouldReturnNotFound()
        {
            var service = new Mock<IPollService>();
            service.Setup(x => x.CheckIfPollExistsAsync(It.IsAny<Guid>())).ReturnsAsync(false);

            var controller = new PollController(service.Object);
            var ret = await controller.Get(Guid.NewGuid());
            ret.Result.Should().BeOfType<NotFoundObjectResult>();
        }

        [Fact]
        public async Task Get_KnownId_ShouldReturnOk()
        {
            var service = new Mock<IPollService>();
            service.Setup(x => x.CheckIfPollExistsAsync(It.IsAny<Guid>())).ReturnsAsync(true);
            service.Setup(x => x.GetPollAsync(It.IsAny<Guid>())).ReturnsAsync(new Poll());

            var controller = new PollController(service.Object);
            var ret = await controller.Get(Guid.NewGuid());
            ret.Result.Should().BeOfType<OkObjectResult>();
        }

        private static PollCreate NewValidPollCreate() => new PollCreate
        {
            Description = "foo",
            EndDate = DateTime.Now.AddDays(7),
            Options = new[] { "a", "b", "c" },
        };

        [Fact]
        public async Task Post_MissingPayload_ShouldReturnBadRequest()
        {
            var controller = new PollController(null);
            var ret = await controller.Post(null);
            ret.Result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task Post_ValidPoll_ShouldReturnOk()
        {
            var poll = NewValidPollCreate();

            var service = new Mock<IPollService>();
            service.Setup(x => x.InsertPollAsync(It.IsAny<PollCreate>())).ReturnsAsync(new Poll());

            var controller = new PollController(service.Object);
            var ret = await controller.Post(poll);
            ret.Result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task Post_MissingDescription_ShouldReturnBadRequest()
        {
            var poll = NewValidPollCreate();
            poll.Description = null;

            var controller = new PollController(null);
            var ret = await controller.Post(poll);
            ret.Result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task Post_LongDescription_ShouldReturnBadRequest()
        {
            var poll = NewValidPollCreate();
            poll.Description = 'a'.Repeat(101).AppendAll();

            var controller = new PollController(null);
            var ret = await controller.Post(poll);
            ret.Result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task Post_MissingEndDate_ShouldReturnBadRequest()
        {
            var poll = NewValidPollCreate();
            poll.EndDate = default;

            var controller = new PollController(null);
            var ret = await controller.Post(poll);
            ret.Result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task Post_MissingOptions_ShouldReturnBadRequest()
        {
            var poll = NewValidPollCreate();
            poll.Options = null;

            var controller = new PollController(null);
            var ret = await controller.Post(poll);
            ret.Result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task Post_FewOptions_ShouldReturnBadRequest()
        {
            var poll = NewValidPollCreate();
            poll.Options = new[] { "foo" };

            var controller = new PollController(null);
            var ret = await controller.Post(poll);
            ret.Result.Should().BeOfType<BadRequestObjectResult>();
        }
    }
}
