using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VSPoll.API.Models.Input;
using VSPoll.API.Models.Output;
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
        /// Gets the voters of an option
        /// </summary>
        /// <param name="query">The query arguments</param>
        /// <returns>The list of voters</returns>
        [Route("voters")]
        [HttpGet]
        [ProducesResponseType(typeof(Page<User>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Page<User>>> GetVoters(VotersQuery query)
        {
            if (query is null)
                return BadRequest("Missing payload");

            if (!ModelState.IsValid)
                return BadRequest("Invalid payload");

            if (!await optionService.CheckIfOptionExistsAsync(query.Option))
                return NotFound("Option doesn't exist");

            var poll = await optionService.GetPollFromOptionAsync(query.Option);
            if (!poll.ShowVoters)
                return Forbid("Poll is anonymous");

            return Ok(await optionService.GetVotersAsync(query));
        }

        /// <summary>
        /// Creates a new option on a poll that allows it
        /// </summary>
        /// <param name="optionCreate">The option data</param>
        /// <returns>The option data</returns>
        [HttpPost]
        [ProducesResponseType(typeof(PollOption), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(string), StatusCodes.Status409Conflict)]
        public async Task<ActionResult<PollOption>> Post(PollOptionCreate optionCreate)
        {
            if (optionCreate is null)
                return BadRequest("Missing payload");

            if (!ModelState.IsValid)
                return BadRequest("Invalid payload");

            if (optionCreate.Poll == default)
                return BadRequest("An option requires a poll");

            if (optionCreate.Description is null)
                return BadRequest("An option requires a description");

            if (optionCreate.Description.Length > 100)
                return BadRequest("Description cannot be longer than 100 characters");

            if (!await pollService.CheckIfPollExistsAsync(optionCreate.Poll))
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
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(string), StatusCodes.Status409Conflict)]
        public async Task<ActionResult> Vote(Vote vote)
        {
            if (vote is null)
                return BadRequest("Missing payload");

            if (!ModelState.IsValid)
                return BadRequest("Invalid payload");

            if (vote.User is null)
                return BadRequest("Missing authentication data");

            if (!userService.Authenticate(vote.User, out var error))
                return Unauthorized(error);

            if (!await optionService.CheckIfOptionExistsAsync(vote.Option))
                return NotFound("Option doesn't exist");

            var poll = await optionService.GetPollFromOptionAsync(vote.Option);
            if (poll.EndDate < DateTime.UtcNow)
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
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(string), StatusCodes.Status409Conflict)]
        public async Task<ActionResult> Unvote(Vote vote)
        {
            if (vote is null)
                return BadRequest("Missing payload");

            if (!ModelState.IsValid)
                return BadRequest("Invalid payload");

            if (vote.User is null)
                return BadRequest("Missing authentication data");

            if (!userService.Authenticate(vote.User, out var error))
                return Unauthorized(error);

            if (!await optionService.CheckIfOptionExistsAsync(vote.Option))
                return NotFound("Option doesn't exist");

            var poll = await optionService.GetPollFromOptionAsync(vote.Option);
            if (poll.EndDate < DateTime.UtcNow)
                return Conflict("This poll has expired");

            var status = await optionService.GetVoteStatusAsync(vote.Option, vote.User.Id);
            if (!status)
                return Ok();

            await optionService.UnvoteAsync(vote.Option, vote.User.Id);
            return Ok();
        }
    }
}
