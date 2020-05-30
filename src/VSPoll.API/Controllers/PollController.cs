﻿using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using VSPoll.API.Extensions;
using VSPoll.API.Models;
using VSPoll.API.Services;

namespace VSPoll.API.Controllers
{
    [Route("poll")]
    [ApiController]
    public class PollController : Controller
    {
        private readonly IPollService pollService;
        public PollController(IPollService pollService)
            => this.pollService = pollService;

        /// <summary>
        /// Gets data from a poll
        /// </summary>
        /// <param name="id">The poll id</param>
        /// <returns>The poll data</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Poll), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<Poll>> Get(Guid id)
        {
            if (!await pollService.CheckIfPollExistsAsync(id))
                return NotFound("Poll doesn't exist");

            return Ok(await pollService.GetPollAsync(id));
        }

        /// <summary>
        /// Creates a new poll
        /// </summary>
        /// <param name="poll">The poll data</param>
        /// <returns>The poll data</returns>
        [HttpPost]
        [ProducesResponseType(typeof(Poll), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<Poll>> Post(PollCreate poll)
        {
            if (poll is null)
                return BadRequest("Missing payload");

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
