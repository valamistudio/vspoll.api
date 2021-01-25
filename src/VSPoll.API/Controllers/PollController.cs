using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VSPoll.API.Extensions;
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
        private readonly IUserService userService;
        public PollController(IPollService pollService, IUserService userService)
        {
            this.pollService = pollService;
            this.userService = userService;
        }

        /// <summary>
        /// Gets data from a poll
        /// </summary>
        /// <param name="id">The poll id</param>
        /// <param name="sort">The sorting algorithm: name, votes (default)</param>
        /// <returns>The poll data</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Poll), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Poll>> Get(Guid id, OptionSorting sort = OptionSorting.Votes)
        {
            if (!ModelState.IsValid)
                return BadRequest("Invalid payload");

            if (!await pollService.CheckIfPollExistsAsync(id))
                return NotFound("Poll doesn't exist");

            return Ok(await pollService.GetPollAsync(id, sort));
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

            return Ok(await pollService.InsertPollAsync(poll));
        }
    }
}
