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

        /// <summary>
        /// Gets data from an option
        /// </summary>
        /// <param name="id">The option id</param>
        /// <returns>The option data</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult> Get(Guid id)
        {
            if (!await optionService.CheckIfOptionExists(id))
                return NotFound("Option doesn't exist");

            return Ok(await optionService.GetOptionAsync(id));
        }

        /// <summary>
        /// Creates a new option on a poll that allows it
        /// </summary>
        /// <param name="optionCreate">The option data</param>
        /// <returns>The option data</returns>
        [HttpPost]
        public async Task<ActionResult> Post(PollOptionCreate optionCreate)
        {
            if (optionCreate.Description.Length > 100)
                return BadRequest("Description cannot be longer than 100 characters");

            if (!await pollService.CheckIfPollExists(optionCreate.Poll))
                return NotFound("Poll doesn't exist");

            var poll = await pollService.GetPollAsync(optionCreate.Poll);
            if (!poll.AllowAdd)
                return Conflict("This poll doesn't allow creating new options");

            if (await optionService.CheckDuplicateAsync(optionCreate))
                return Conflict("Option already exists");

            return Ok(await optionService.InsertOptionAsync(optionCreate));
        }

        /// <summary>
        /// Cast a vote on an option
        /// </summary>
        /// <param name="vote">The vote data</param>
        [Route("vote")]
        [HttpPost]
        public async Task<ActionResult> Vote(Vote vote)
        {
            if (!userService.Authenticate(vote.User, out var error))
                return Unauthorized(error);

            if (!await optionService.CheckIfOptionExists(vote.Option))
                return NotFound("Option doesn't exist");

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

        /// <summary>
        /// Uncast a vote on an option
        /// </summary>
        /// <param name="vote">the vote data</param>
        [Route("vote")]
        [HttpDelete]
        public async Task<ActionResult> Unvote(Vote vote)
        {
            if (!userService.Authenticate(vote.User, out var error))
                return Unauthorized(error);

            if (!await optionService.CheckIfOptionExists(vote.Option))
                return NotFound("Option doesn't exist");

            var status = await optionService.GetVoteStatusAsync(vote.Option, vote.User.Id);
            if (!status)
                return NotFound("User hasn't voted this option");

            await optionService.UnvoteAsync(vote.Option, vote.User.Id);
            return Ok();
        }
    }
}
