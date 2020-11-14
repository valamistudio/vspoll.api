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
    public class OptionControllerTests
    {
        [Fact]
        public async Task GetVoters_MissingPayload_ShouldReturnBadRequest()
        {
            OptionController controller = new(null, null, null);
            var ret = await controller.GetVoters(null);
            ret.Result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task GetVoters_UnknownOption_ShouldReturnNotFound()
        {
            VotersQuery query = new();

            Mock<IOptionService> optionService = new();
            optionService.Setup(x => x.CheckIfOptionExistsAsync(It.IsAny<Guid>())).ReturnsAsync(false);

            OptionController controller = new(null, optionService.Object, null);
            var ret = await controller.GetVoters(query);
            ret.Result.Should().BeOfType<NotFoundObjectResult>();
        }

        [Fact]
        public async Task GetVoters_AnonymousPoll_ShouldReturnForbidden()
        {
            VotersQuery query = new();

            Mock<IOptionService> optionService = new();
            optionService.Setup(x => x.CheckIfOptionExistsAsync(It.IsAny<Guid>())).ReturnsAsync(true);
            optionService.Setup(x => x.GetPollFromOptionAsync(It.IsAny<Guid>())).ReturnsAsync(new Poll());

            OptionController controller = new(null, optionService.Object, null);
            var ret = await controller.GetVoters(query);
            ret.Result.Should().BeOfType<ForbidResult>();
        }

        [Fact]
        public async Task GetVoters_ValidInput_ShouldReturnOk()
        {
            VotersQuery query = new();

            Mock<IOptionService> optionService = new();
            optionService.Setup(x => x.CheckIfOptionExistsAsync(It.IsAny<Guid>())).ReturnsAsync(true);
            optionService.Setup(x => x.GetPollFromOptionAsync(It.IsAny<Guid>())).ReturnsAsync(new Poll { ShowVoters = true });
            optionService.Setup(x => x.GetVotersAsync(It.IsAny<VotersQuery>())).ReturnsAsync(new Page<User>(1, 1, 0, Enumerable.Empty<User>()));

            OptionController controller = new(null, optionService.Object, null);
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
            OptionController controller = new(null, null, null);
            var ret = await controller.Post(null);
            ret.Result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task Post_MissingDescription_ShouldReturnBadRequest()
        {
            var option = NewValidPollOptionCreate();
            option.Description = null;

            OptionController controller = new(null, null, null);
            var ret = await controller.Post(option);
            ret.Result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task Post_LongDescription_ShouldReturnBadRequest()
        {
            var option = NewValidPollOptionCreate();
            option.Description = 'a'.Repeat(101).AppendAll();

            OptionController controller = new(null, null, null);
            var ret = await controller.Post(option);
            ret.Result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task Post_UnknownPoll_ShouldReturnNotFound()
        {
            var option = NewValidPollOptionCreate();

            Mock<IPollService> pollService = new();
            pollService.Setup(x => x.CheckIfPollExistsAsync(It.IsAny<Guid>())).ReturnsAsync(false);

            OptionController controller = new(pollService.Object, null, null);
            var ret = await controller.Post(option);
            ret.Result.Should().BeOfType<NotFoundObjectResult>();
        }

        [Fact]
        public async Task Post_PollDoesntAllowNewOptions_ShouldReturnConflict()
        {
            var option = NewValidPollOptionCreate();

            Mock<IPollService> pollService = new();
            pollService.Setup(x => x.CheckIfPollExistsAsync(It.IsAny<Guid>())).ReturnsAsync(true);
            pollService.Setup(x => x.GetPollAsync(It.IsAny<Guid>())).ReturnsAsync(new Poll());

            OptionController controller = new(pollService.Object, null, null);
            var ret = await controller.Post(option);
            ret.Result.Should().BeOfType<ConflictObjectResult>();
        }

        [Fact]
        public async Task Post_DuplicateOption_ShouldReturnConflict()
        {
            var option = NewValidPollOptionCreate();

            Mock<IPollService> pollService = new();
            pollService.Setup(x => x.CheckIfPollExistsAsync(It.IsAny<Guid>())).ReturnsAsync(true);
            pollService.Setup(x => x.GetPollAsync(It.IsAny<Guid>())).ReturnsAsync(new Poll { AllowAdd = true });

            Mock<IOptionService> optionService = new();
            optionService.Setup(x => x.CheckDuplicateAsync(It.IsAny<PollOptionCreate>())).ReturnsAsync(true);

            OptionController controller = new(pollService.Object, optionService.Object, null);
            var ret = await controller.Post(option);
            ret.Result.Should().BeOfType<ConflictObjectResult>();
        }

        [Fact]
        public async Task Post_ValidOption_ShouldReturnOk()
        {
            var option = NewValidPollOptionCreate();

            Mock<IPollService> pollService = new();
            pollService.Setup(x => x.CheckIfPollExistsAsync(It.IsAny<Guid>())).ReturnsAsync(true);
            pollService.Setup(x => x.GetPollAsync(It.IsAny<Guid>())).ReturnsAsync(new Poll { AllowAdd = true });
            pollService.Setup(x => x.InsertPollAsync(It.IsAny<PollCreate>())).ReturnsAsync(new Poll());

            Mock<IOptionService> optionService = new();
            optionService.Setup(x => x.CheckDuplicateAsync(It.IsAny<PollOptionCreate>())).ReturnsAsync(false);

            OptionController controller = new(pollService.Object, optionService.Object, null);
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
            OptionController controller = new(null, null, null);
            var ret = await controller.Vote(null);
            ret.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task Vote_MissingAuthentication_ShouldReturnBadRequest()
        {
            var vote = NewValidVote();
            vote.User = null;

            OptionController controller = new(null, null, null);
            var ret = await controller.Vote(vote);
            ret.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task Vote_FailedAuthentication_ShouldReturnUnauthorized()
        {
            var vote = NewValidVote();

            Mock<IUserService> userService = new();
            userService.Setup(x => x.Authenticate(It.IsAny<Authentication>(), out It.Ref<string>.IsAny)).Returns(false);

            OptionController controller = new(null, null, userService.Object);
            var ret = await controller.Vote(vote);
            ret.Should().BeOfType<UnauthorizedObjectResult>();
        }

        [Fact]
        public async Task Vote_UnknownOption_ShouldReturnNotFound()
        {
            var vote = NewValidVote();

            Mock<IOptionService> optionService = new();
            optionService.Setup(x => x.CheckIfOptionExistsAsync(It.IsAny<Guid>())).ReturnsAsync(false);

            Mock<IUserService> userService = new();
            userService.Setup(x => x.Authenticate(It.IsAny<Authentication>(), out It.Ref<string>.IsAny)).Returns(true);

            OptionController controller = new(null, optionService.Object, userService.Object);
            var ret = await controller.Vote(vote);
            ret.Should().BeOfType<NotFoundObjectResult>();
        }

        [Fact]
        public async Task Vote_PollExpired_ShouldReturnConflict()
        {
            var vote = NewValidVote();

            Mock<IOptionService> optionService = new();
            optionService.Setup(x => x.CheckIfOptionExistsAsync(It.IsAny<Guid>())).ReturnsAsync(true);
            optionService.Setup(x => x.GetPollFromOptionAsync(It.IsAny<Guid>())).ReturnsAsync(new Poll());

            Mock<IUserService> userService = new();
            userService.Setup(x => x.Authenticate(It.IsAny<Authentication>(), out It.Ref<string>.IsAny)).Returns(true);

            OptionController controller = new(null, optionService.Object, userService.Object);
            var ret = await controller.Vote(vote);
            ret.Should().BeOfType<ConflictObjectResult>();
        }

        [Fact]
        public async Task Vote_AlreadyVoted_ShouldReturnOk()
        {
            var vote = NewValidVote();

            Mock<IOptionService> optionService = new();
            optionService.Setup(x => x.CheckIfOptionExistsAsync(It.IsAny<Guid>())).ReturnsAsync(true);
            optionService.Setup(x => x.GetPollFromOptionAsync(It.IsAny<Guid>())).ReturnsAsync(new Poll { EndDate = DateTime.UtcNow.AddDays(7) });
            optionService.Setup(x => x.GetVoteStatusAsync(It.IsAny<Guid>(), It.IsAny<int>())).ReturnsAsync(true);

            Mock<IUserService> userService = new();
            userService.Setup(x => x.Authenticate(It.IsAny<Authentication>(), out It.Ref<string>.IsAny)).Returns(true);

            OptionController controller = new(null, optionService.Object, userService.Object);
            var ret = await controller.Vote(vote);
            ret.Should().BeOfType<OkResult>();
        }

        [Fact]
        public async Task Vote_NotVoted_ShouldReturnOk()
        {
            var vote = NewValidVote();

            Mock<IOptionService> optionService = new();
            optionService.Setup(x => x.CheckIfOptionExistsAsync(It.IsAny<Guid>())).ReturnsAsync(true);
            optionService.Setup(x => x.GetPollFromOptionAsync(It.IsAny<Guid>())).ReturnsAsync(new Poll { EndDate = DateTime.UtcNow.AddDays(7) });
            optionService.Setup(x => x.GetVoteStatusAsync(It.IsAny<Guid>(), It.IsAny<int>())).ReturnsAsync(false);
            optionService.Setup(x => x.ClearVoteAsync(It.IsAny<Guid>(), It.IsAny<int>())).Returns(Task.CompletedTask);
            optionService.Setup(x => x.VoteAsync(It.IsAny<Guid>(), It.IsAny<int>())).Returns(Task.CompletedTask);

            Mock<IUserService> userService = new();
            userService.Setup(x => x.Authenticate(It.IsAny<Authentication>(), out It.Ref<string>.IsAny)).Returns(true);
            userService.Setup(x => x.AddOrUpdateUserAsync(It.IsAny<Authentication>())).Returns(Task.CompletedTask);

            OptionController controller = new(null, optionService.Object, userService.Object);
            var ret = await controller.Vote(vote);
            ret.Should().BeOfType<OkResult>();
        }

        [Fact]
        public async Task Unvote_MissingPayload_ShouldReturnBadRequest()
        {
            OptionController controller = new(null, null, null);
            var ret = await controller.Unvote(null);
            ret.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task Unvote_MissingAuthentication_ShouldReturnBadRequest()
        {
            var vote = NewValidVote();
            vote.User = null;

            OptionController controller = new(null, null, null);
            var ret = await controller.Unvote(vote);
            ret.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task Unvote_FailedAuthentication_ShouldReturnUnauthorized()
        {
            var vote = NewValidVote();

            Mock<IUserService> userService = new();
            userService.Setup(x => x.Authenticate(It.IsAny<Authentication>(), out It.Ref<string>.IsAny)).Returns(false);

            OptionController controller = new(null, null, userService.Object);
            var ret = await controller.Unvote(vote);
            ret.Should().BeOfType<UnauthorizedObjectResult>();
        }

        [Fact]
        public async Task Unvote_UnknownOption_ShouldReturnNotFound()
        {
            var vote = NewValidVote();

            Mock<IOptionService> optionService = new();
            optionService.Setup(x => x.CheckIfOptionExistsAsync(It.IsAny<Guid>())).ReturnsAsync(false);

            Mock<IUserService> userService = new();
            userService.Setup(x => x.Authenticate(It.IsAny<Authentication>(), out It.Ref<string>.IsAny)).Returns(true);

            OptionController controller = new(null, optionService.Object, userService.Object);
            var ret = await controller.Unvote(vote);
            ret.Should().BeOfType<NotFoundObjectResult>();
        }

        [Fact]
        public async Task Unvote_PollExpired_ShouldReturnConflict()
        {
            var vote = NewValidVote();

            Mock<IOptionService> optionService = new();
            optionService.Setup(x => x.CheckIfOptionExistsAsync(It.IsAny<Guid>())).ReturnsAsync(true);
            optionService.Setup(x => x.GetPollFromOptionAsync(It.IsAny<Guid>())).ReturnsAsync(new Poll());

            Mock<IUserService> userService = new();
            userService.Setup(x => x.Authenticate(It.IsAny<Authentication>(), out It.Ref<string>.IsAny)).Returns(true);

            OptionController controller = new(null, optionService.Object, userService.Object);
            var ret = await controller.Unvote(vote);
            ret.Should().BeOfType<ConflictObjectResult>();
        }

        [Fact]
        public async Task Unvote_NotVoted_ShouldReturnOk()
        {
            var vote = NewValidVote();

            Mock<IOptionService> optionService = new();
            optionService.Setup(x => x.CheckIfOptionExistsAsync(It.IsAny<Guid>())).ReturnsAsync(true);
            optionService.Setup(x => x.GetPollFromOptionAsync(It.IsAny<Guid>())).ReturnsAsync(new Poll { EndDate = DateTime.UtcNow.AddDays(7) });
            optionService.Setup(x => x.GetVoteStatusAsync(It.IsAny<Guid>(), It.IsAny<int>())).ReturnsAsync(false);

            Mock<IUserService> userService = new();
            userService.Setup(x => x.Authenticate(It.IsAny<Authentication>(), out It.Ref<string>.IsAny)).Returns(true);

            OptionController controller = new(null, optionService.Object, userService.Object);
            var ret = await controller.Unvote(vote);
            ret.Should().BeOfType<OkResult>();
        }

        [Fact]
        public async Task Unvote_Voted_ShouldReturnOk()
        {
            var vote = NewValidVote();

            Mock<IOptionService> optionService = new();
            optionService.Setup(x => x.CheckIfOptionExistsAsync(It.IsAny<Guid>())).ReturnsAsync(true);
            optionService.Setup(x => x.GetPollFromOptionAsync(It.IsAny<Guid>())).ReturnsAsync(new Poll { EndDate = DateTime.UtcNow.AddDays(7) });
            optionService.Setup(x => x.GetVoteStatusAsync(It.IsAny<Guid>(), It.IsAny<int>())).ReturnsAsync(true);
            optionService.Setup(x => x.UnvoteAsync(It.IsAny<Guid>(), It.IsAny<int>())).Returns(Task.CompletedTask);

            Mock<IUserService> userService = new();
            userService.Setup(x => x.Authenticate(It.IsAny<Authentication>(), out It.Ref<string>.IsAny)).Returns(true);

            OptionController controller = new(null, optionService.Object, userService.Object);
            var ret = await controller.Unvote(vote);
            ret.Should().BeOfType<OkResult>();
        }
    }
}
