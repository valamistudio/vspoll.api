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
    public class OptionControllerTests
    {
        [Fact]
        public async Task GetVoters_MissingPayload_ShouldReturnBadRequest()
        {
            var controller = new OptionController(null, null, null);
            var ret = await controller.GetVoters(null);
            ret.Result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task GetVoters_UnknownOption_ShouldReturnNotFound()
        {
            var query = new VotersQuery();

            var optionService = new Mock<IOptionService>();
            optionService.Setup(x => x.CheckIfOptionExistsAsync(It.IsAny<Guid>())).ReturnsAsync(false);

            var controller = new OptionController(null, optionService.Object, null);
            var ret = await controller.GetVoters(query);
            ret.Result.Should().BeOfType<NotFoundObjectResult>();
        }

        [Fact]
        public async Task GetVoters_KnownOption_ShouldReturnOk()
        {
            var query = new VotersQuery();

            var optionService = new Mock<IOptionService>();
            optionService.Setup(x => x.CheckIfOptionExistsAsync(It.IsAny<Guid>())).ReturnsAsync(true);

            var controller = new OptionController(null, optionService.Object, null);
            var ret = await controller.GetVoters(query);
            ret.Result.Should().BeOfType<OkObjectResult>();
        }

        private static PollOptionCreate NewValidPollOptionCreate() => new PollOptionCreate
        {
            Description = "foo",
            Poll = Guid.NewGuid(),
        };

        [Fact]
        public async Task Post_MissingPayload_ShouldReturnBadRequest()
        {
            var controller = new OptionController(null, null, null);
            var ret = await controller.Post(null);
            ret.Result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task Post_MissingDescription_ShouldReturnBadRequest()
        {
            var option = NewValidPollOptionCreate();
            option.Description = null;

            var controller = new OptionController(null, null, null);
            var ret = await controller.Post(option);
            ret.Result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task Post_LongDescription_ShouldReturnBadRequest()
        {
            var option = NewValidPollOptionCreate();
            option.Description = 'a'.Repeat(101).AppendAll();

            var controller = new OptionController(null, null, null);
            var ret = await controller.Post(option);
            ret.Result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task Post_UnknownPoll_ShouldReturnNotFound()
        {
            var option = NewValidPollOptionCreate();

            var pollService = new Mock<IPollService>();
            pollService.Setup(x => x.CheckIfPollExistsAsync(It.IsAny<Guid>())).ReturnsAsync(false);

            var controller = new OptionController(pollService.Object, null, null);
            var ret = await controller.Post(option);
            ret.Result.Should().BeOfType<NotFoundObjectResult>();
        }

        [Fact]
        public async Task Post_PollDoesntAllowNewOptions_ShouldReturnConflict()
        {
            var option = NewValidPollOptionCreate();

            var pollService = new Mock<IPollService>();
            pollService.Setup(x => x.CheckIfPollExistsAsync(It.IsAny<Guid>())).ReturnsAsync(true);
            pollService.Setup(x => x.GetPollAsync(It.IsAny<Guid>())).ReturnsAsync(new Poll());

            var controller = new OptionController(pollService.Object, null, null);
            var ret = await controller.Post(option);
            ret.Result.Should().BeOfType<ConflictObjectResult>();
        }

        [Fact]
        public async Task Post_DuplicateOption_ShouldReturnConflict()
        {
            var option = NewValidPollOptionCreate();

            var pollService = new Mock<IPollService>();
            pollService.Setup(x => x.CheckIfPollExistsAsync(It.IsAny<Guid>())).ReturnsAsync(true);
            pollService.Setup(x => x.GetPollAsync(It.IsAny<Guid>())).ReturnsAsync(new Poll { AllowAdd = true });

            var optionService = new Mock<IOptionService>();
            optionService.Setup(x => x.CheckDuplicateAsync(It.IsAny<PollOptionCreate>())).ReturnsAsync(true);

            var controller = new OptionController(pollService.Object, optionService.Object, null);
            var ret = await controller.Post(option);
            ret.Result.Should().BeOfType<ConflictObjectResult>();
        }

        [Fact]
        public async Task Post_ValidOption_ShouldReturnOk()
        {
            var option = NewValidPollOptionCreate();

            var pollService = new Mock<IPollService>();
            pollService.Setup(x => x.CheckIfPollExistsAsync(It.IsAny<Guid>())).ReturnsAsync(true);
            pollService.Setup(x => x.GetPollAsync(It.IsAny<Guid>())).ReturnsAsync(new Poll { AllowAdd = true });
            pollService.Setup(x => x.InsertPollAsync(It.IsAny<PollCreate>())).ReturnsAsync(new Poll());

            var optionService = new Mock<IOptionService>();
            optionService.Setup(x => x.CheckDuplicateAsync(It.IsAny<PollOptionCreate>())).ReturnsAsync(false);

            var controller = new OptionController(pollService.Object, optionService.Object, null);
            var ret = await controller.Post(option);
            ret.Result.Should().BeOfType<OkObjectResult>();
        }

        private static Vote NewValidVote() => new Vote
        {
            Option = Guid.NewGuid(),
            User = new Authentication(),
        };

        [Fact]
        public async Task Vote_MissingPayload_ShouldReturnBadRequest()
        {
            var controller = new OptionController(null, null, null);
            var ret = await controller.Vote(null);
            ret.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task Vote_MissingAuthentication_ShouldReturnBadRequest()
        {
            var vote = NewValidVote();
            vote.User = null;

            var controller = new OptionController(null, null, null);
            var ret = await controller.Vote(vote);
            ret.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task Vote_FailedAuthentication_ShouldReturnUnauthorized()
        {
            var vote = NewValidVote();

            var userService = new Mock<IUserService>();
            userService.Setup(x => x.Authenticate(It.IsAny<Authentication>(), out It.Ref<string>.IsAny)).Returns(false);

            var controller = new OptionController(null, null, userService.Object);
            var ret = await controller.Vote(vote);
            ret.Should().BeOfType<UnauthorizedObjectResult>();
        }

        [Fact]
        public async Task Vote_UnknownOption_ShouldReturnNotFound()
        {
            var vote = NewValidVote();

            var optionService = new Mock<IOptionService>();
            optionService.Setup(x => x.CheckIfOptionExistsAsync(It.IsAny<Guid>())).ReturnsAsync(false);

            var userService = new Mock<IUserService>();
            userService.Setup(x => x.Authenticate(It.IsAny<Authentication>(), out It.Ref<string>.IsAny)).Returns(true);

            var controller = new OptionController(null, optionService.Object, userService.Object);
            var ret = await controller.Vote(vote);
            ret.Should().BeOfType<NotFoundObjectResult>();
        }

        [Fact]
        public async Task Vote_PollExpired_ShouldReturnConflict()
        {
            var vote = NewValidVote();

            var optionService = new Mock<IOptionService>();
            optionService.Setup(x => x.CheckIfOptionExistsAsync(It.IsAny<Guid>())).ReturnsAsync(true);
            optionService.Setup(x => x.GetPollFromOptionAsync(It.IsAny<Guid>())).ReturnsAsync(new Poll());

            var userService = new Mock<IUserService>();
            userService.Setup(x => x.Authenticate(It.IsAny<Authentication>(), out It.Ref<string>.IsAny)).Returns(true);

            var controller = new OptionController(null, optionService.Object, userService.Object);
            var ret = await controller.Vote(vote);
            ret.Should().BeOfType<ConflictObjectResult>();
        }

        [Fact]
        public async Task Vote_AlreadyVoted_ShouldReturnOk()
        {
            var vote = NewValidVote();

            var optionService = new Mock<IOptionService>();
            optionService.Setup(x => x.CheckIfOptionExistsAsync(It.IsAny<Guid>())).ReturnsAsync(true);
            optionService.Setup(x => x.GetPollFromOptionAsync(It.IsAny<Guid>())).ReturnsAsync(new Poll { EndDate = DateTime.Now.AddDays(7) });
            optionService.Setup(x => x.GetVoteStatusAsync(It.IsAny<Guid>(), It.IsAny<int>())).ReturnsAsync(true);

            var userService = new Mock<IUserService>();
            userService.Setup(x => x.Authenticate(It.IsAny<Authentication>(), out It.Ref<string>.IsAny)).Returns(true);

            var controller = new OptionController(null, optionService.Object, userService.Object);
            var ret = await controller.Vote(vote);
            ret.Should().BeOfType<OkResult>();
        }

        [Fact]
        public async Task Vote_NotVoted_ShouldReturnOk()
        {
            var vote = NewValidVote();

            var optionService = new Mock<IOptionService>();
            optionService.Setup(x => x.CheckIfOptionExistsAsync(It.IsAny<Guid>())).ReturnsAsync(true);
            optionService.Setup(x => x.GetPollFromOptionAsync(It.IsAny<Guid>())).ReturnsAsync(new Poll { EndDate = DateTime.Now.AddDays(7) });
            optionService.Setup(x => x.GetVoteStatusAsync(It.IsAny<Guid>(), It.IsAny<int>())).ReturnsAsync(false);
            optionService.Setup(x => x.ClearVoteAsync(It.IsAny<Guid>(), It.IsAny<int>())).Returns(Task.CompletedTask);
            optionService.Setup(x => x.VoteAsync(It.IsAny<Guid>(), It.IsAny<int>())).Returns(Task.CompletedTask);

            var userService = new Mock<IUserService>();
            userService.Setup(x => x.Authenticate(It.IsAny<Authentication>(), out It.Ref<string>.IsAny)).Returns(true);
            userService.Setup(x => x.AddOrUpdateUserAsync(It.IsAny<Authentication>())).Returns(Task.CompletedTask);

            var controller = new OptionController(null, optionService.Object, userService.Object);
            var ret = await controller.Vote(vote);
            ret.Should().BeOfType<OkResult>();
        }

        [Fact]
        public async Task Unvote_MissingPayload_ShouldReturnBadRequest()
        {
            var controller = new OptionController(null, null, null);
            var ret = await controller.Unvote(null);
            ret.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task Unvote_MissingAuthentication_ShouldReturnBadRequest()
        {
            var vote = NewValidVote();
            vote.User = null;

            var controller = new OptionController(null, null, null);
            var ret = await controller.Unvote(vote);
            ret.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task Unvote_FailedAuthentication_ShouldReturnUnauthorized()
        {
            var vote = NewValidVote();

            var userService = new Mock<IUserService>();
            userService.Setup(x => x.Authenticate(It.IsAny<Authentication>(), out It.Ref<string>.IsAny)).Returns(false);

            var controller = new OptionController(null, null, userService.Object);
            var ret = await controller.Unvote(vote);
            ret.Should().BeOfType<UnauthorizedObjectResult>();
        }

        [Fact]
        public async Task Unvote_UnknownOption_ShouldReturnNotFound()
        {
            var vote = NewValidVote();

            var optionService = new Mock<IOptionService>();
            optionService.Setup(x => x.CheckIfOptionExistsAsync(It.IsAny<Guid>())).ReturnsAsync(false);

            var userService = new Mock<IUserService>();
            userService.Setup(x => x.Authenticate(It.IsAny<Authentication>(), out It.Ref<string>.IsAny)).Returns(true);

            var controller = new OptionController(null, optionService.Object, userService.Object);
            var ret = await controller.Unvote(vote);
            ret.Should().BeOfType<NotFoundObjectResult>();
        }

        [Fact]
        public async Task Unvote_PollExpired_ShouldReturnConflict()
        {
            var vote = NewValidVote();

            var optionService = new Mock<IOptionService>();
            optionService.Setup(x => x.CheckIfOptionExistsAsync(It.IsAny<Guid>())).ReturnsAsync(true);
            optionService.Setup(x => x.GetPollFromOptionAsync(It.IsAny<Guid>())).ReturnsAsync(new Poll());

            var userService = new Mock<IUserService>();
            userService.Setup(x => x.Authenticate(It.IsAny<Authentication>(), out It.Ref<string>.IsAny)).Returns(true);

            var controller = new OptionController(null, optionService.Object, userService.Object);
            var ret = await controller.Unvote(vote);
            ret.Should().BeOfType<ConflictObjectResult>();
        }

        [Fact]
        public async Task Unvote_NotVoted_ShouldReturnOk()
        {
            var vote = NewValidVote();

            var optionService = new Mock<IOptionService>();
            optionService.Setup(x => x.CheckIfOptionExistsAsync(It.IsAny<Guid>())).ReturnsAsync(true);
            optionService.Setup(x => x.GetPollFromOptionAsync(It.IsAny<Guid>())).ReturnsAsync(new Poll { EndDate = DateTime.Now.AddDays(7) });
            optionService.Setup(x => x.GetVoteStatusAsync(It.IsAny<Guid>(), It.IsAny<int>())).ReturnsAsync(false);

            var userService = new Mock<IUserService>();
            userService.Setup(x => x.Authenticate(It.IsAny<Authentication>(), out It.Ref<string>.IsAny)).Returns(true);

            var controller = new OptionController(null, optionService.Object, userService.Object);
            var ret = await controller.Unvote(vote);
            ret.Should().BeOfType<OkResult>();
        }

        [Fact]
        public async Task Unvote_Voted_ShouldReturnOk()
        {
            var vote = NewValidVote();

            var optionService = new Mock<IOptionService>();
            optionService.Setup(x => x.CheckIfOptionExistsAsync(It.IsAny<Guid>())).ReturnsAsync(true);
            optionService.Setup(x => x.GetPollFromOptionAsync(It.IsAny<Guid>())).ReturnsAsync(new Poll { EndDate = DateTime.Now.AddDays(7) });
            optionService.Setup(x => x.GetVoteStatusAsync(It.IsAny<Guid>(), It.IsAny<int>())).ReturnsAsync(true);
            optionService.Setup(x => x.UnvoteAsync(It.IsAny<Guid>(), It.IsAny<int>())).Returns(Task.CompletedTask);

            var userService = new Mock<IUserService>();
            userService.Setup(x => x.Authenticate(It.IsAny<Authentication>(), out It.Ref<string>.IsAny)).Returns(true);

            var controller = new OptionController(null, optionService.Object, userService.Object);
            var ret = await controller.Unvote(vote);
            ret.Should().BeOfType<OkResult>();
        }
    }
}
