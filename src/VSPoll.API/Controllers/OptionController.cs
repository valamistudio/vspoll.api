using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using VSPoll.API.Models;
using VSPoll.API.Services;

namespace VSPoll.API.Controllers
{
    [Route("option")]
    [ApiController]
    public class OptionController : Controller
    {
        private readonly IPollService pollService;
        private readonly IOptionService optionService;
        private readonly IUserService userService;
        public OptionController(IPollService pollService, IOptionService optionService, IUserService userService)
        {
            this.pollService = pollService;
            this.optionService = optionService;
            this.userService = userService;
        }

        [HttpPost]
        public async Task<ActionResult> Post(PollOptionCreate optionCreate)
        {
            var poll = await pollService.GetPollAsync(optionCreate.Poll);
            if (!poll.AllowAdd)
                return Conflict("This poll doesn't allow creating new options");

            if (await optionService.CheckDuplicateAsync(optionCreate))
                return Conflict("Option already exists");

            return Ok(await optionService.InsertOptionAsync(optionCreate));
        }

        [Route("vote")]
        [HttpPost]
        public async Task<ActionResult> Vote(Vote vote)
        {
            if (!userService.Authenticate(vote.User, out var error))
                return Unauthorized(error);

            var poll = await optionService.GetPollFromOptionAsync(vote.Option);
            if (poll.EndDate < DateTime.Now)
                return Conflict("This poll has expired");

            var status = await optionService.GetVoteStatusAsync(vote.Option, vote.User.Id);
            if (status)
                return Ok();

            await userService.AddOrUpdateUserAsync(vote.User);

            if (!poll.MultiVote)
                await optionService.ClearVoteAsync(poll.Id, vote.User.Id);

            await optionService.VoteAsync(vote.Option, vote.User.Id);
            return Ok();
        }

        [Route("vote")]
        [HttpDelete]
        public async Task<ActionResult> Unvote(Vote vote)
        {
            if (!userService.Authenticate(vote.User, out var error))
                return Unauthorized(error);

            var status = await optionService.GetVoteStatusAsync(vote.Option, vote.User.Id);
            if (!status)
                return NotFound("User hasn't voted this option");

            await optionService.UnvoteAsync(vote.Option, vote.User.Id);
            return Ok();
        }
    }
}
