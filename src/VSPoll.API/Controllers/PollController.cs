using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VSPoll.API.Extensions;
using VSPoll.API.Models;
using VSPoll.API.Models.Input;
using VSPoll.API.Models.Output;
using VSPoll.API.Services;

namespace VSPoll.API.Controllers
{
    [Route("poll")]
    [ApiController]
    public class PollController : Controller
    {
        private readonly IPollService pollService;
        private readonly IOptionService optionService;
        private readonly IUserService userService;
        public PollController(IPollService pollService, IOptionService optionService, IUserService userService)
        {
            this.pollService = pollService;
            this.optionService = optionService;
            this.userService = userService;
        }

        /// <summary>
        /// Gets data from a poll
        /// </summary>
        /// <param name="id">The poll id</param>
        /// <param name="sort">The sorting algorithm: name, votes (default)</param>
        /// <returns>The poll data</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(PollView), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PollView>> Get(Guid id, OptionSorting sort = OptionSorting.Votes)
        {
            if (!ModelState.IsValid)
                return BadRequest("Invalid payload");

            var poll = await pollService.GetPollViewAsync(id, sort);
            if (poll is null)
                return NotFound("Poll doesn't exist");

            return Ok(poll);
        }

        /// <summary>
        /// Gets the vote(s) of the authenticated user
        /// </summary>
        /// <param name="id">The poll id</param>
        /// <param name="authentication">The authentication data</param>
        /// <returns>A collection of option ids</returns>
        [HttpGet("{id}/votes")]
        [ProducesResponseType(typeof(IEnumerable<Guid>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<Guid>>> GetVotes(Guid id, [FromBody] Authentication authentication)
        {
            if (authentication is null)
                return BadRequest("Missing authentication payload");

            if (!ModelState.IsValid)
                return BadRequest("Invalid payload");

            if (!userService.Authenticate(authentication, out var error))
                return Unauthorized(error);

            if (!await pollService.CheckIfPollExistsAsync(id))
                return NotFound("Poll doesn't exist");

            return Ok(pollService.GetVotes(id, authentication.Id));
        }

        /// <summary>
        /// Creates a new poll
        /// </summary>
        /// <param name="poll">The poll data</param>
        /// <returns>The poll data</returns>
        [HttpPost]
        [ProducesResponseType(typeof(Poll), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Poll>> Post([FromBody] PollCreate poll)
        {
            if (poll is null)
                return BadRequest("Missing payload");

            if (!ModelState.IsValid)
                return BadRequest("Invalid payload");

            if (poll.Description is null)
                return BadRequest("A poll requires a description");

            if (poll.EndDate == default)
                return BadRequest("A poll required an ending date");

            if (poll.Description.Length > 100)
                return BadRequest("Description cannot be longer than 100 characters");

            if (poll.Options is null || !poll.Options.AtLeast(2))
                return BadRequest("A poll requires at least two options");

            if (poll.VotingSystem == VotingSystem.Ranked)
            {
                if (poll.AllowAdd)
                    return BadRequest("A ranked system poll cannot have more options added after it's been created");

                if (poll.ShowVoters)
                    return BadRequest("A ranked system poll cannot show its voters");
            }
            return Ok(await pollService.InsertPollAsync(poll));
        }

        /// <summary>
        /// Cast a vote on a ranked poll<br/>
        /// The order of the options is important if the poll's voting system is ranked
        /// </summary>
        /// <param name="id">The poll id</param>
        /// <param name="vote">The voting and authentication data</param>
        [HttpPost("{id}/vote")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(string), StatusCodes.Status409Conflict)]
        public async Task<ActionResult> Vote(Guid id, [FromBody] RankedVote vote)
        {
            if (vote is null)
                return BadRequest("Missing payload");

            if (!ModelState.IsValid)
                return BadRequest("Invalid payload");

            if (vote.User is null)
                return BadRequest("Missing authentication data");

            if (!userService.Authenticate(vote.User, out var error))
                return Unauthorized(error);

            var poll = await pollService.GetPollAsync(id);
            if (poll is null)
                return NotFound("Poll doesn't exist");

            if (poll.EndDate < DateTime.UtcNow)
                return Conflict("This poll has expired");

            if (vote.Options is null || !vote.Options.Any())
                if (poll.VotingSystem == VotingSystem.SingleOption)
                    return BadRequest("An option should be inputted");
                else
                    return BadRequest("At least one option should be inputted");

            if (poll.VotingSystem == VotingSystem.Ranked)
            {
                if (!vote.Options.Count(vote.Options.Distinct().Count()))
                    return BadRequest("Cannot cast duplicate votes on a ranked system poll");
            }
            else
            {
                vote.Options = vote.Options.Distinct();
                if (poll.VotingSystem == VotingSystem.SingleOption && vote.Options.AtLeast(2))
                    return BadRequest("This poll voting system is single option, only one vote can be cast");
            }
            foreach (var optionId in vote.Options)
            {
                var option = await optionService.GetOptionAsync(optionId);
                if (option is null)
                    return NotFound("Option doesn't exist");

                if (option.Poll != id)
                    return NotFound("Option doesn't exist in this poll");
            }
            await pollService.VoteAsync(poll, vote.User.Id, vote.Options);
            return Ok();
        }

        /// <summary>
        /// Uncast a vote on a poll
        /// </summary>
        /// <param name="id">The poll id</param>
        /// <param name="authentication">The authentication data</param>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(string), StatusCodes.Status409Conflict)]
        public async Task<ActionResult> Unvote(Guid id, [FromBody] Authentication authentication)
        {
            if (authentication is null)
                return BadRequest("Missing authentication payload");

            if (!ModelState.IsValid)
                return BadRequest("Invalid payload");

            if (!userService.Authenticate(authentication, out var error))
                return Unauthorized(error);

            var poll = await pollService.GetPollAsync(id);
            if (poll is null)
                return NotFound("Poll doesn't exist");

            if (poll.EndDate < DateTime.UtcNow)
                return Conflict("This poll has expired");

            await pollService.UnvoteAsync(id, authentication.Id);
            return Ok();
        }
    }
}
