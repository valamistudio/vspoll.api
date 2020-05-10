using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
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

        [HttpGet("{id}")]
        public async Task<ActionResult<Poll>> Get(Guid id)
        {
            if (!await pollService.CheckIfPollExists(id))
                return NotFound("Poll doesn't exist");

            return Ok(await pollService.GetPollAsync(id));
        }

        [HttpPost]
        public async Task<ActionResult<Poll>> Post(PollCreate poll)
        {
            if (poll.Description.Length > 100)
                return BadRequest("Description cannot be longer than 100 characters");

            return Ok(await pollService.InsertPollAsync(poll));
        }
    }
}
