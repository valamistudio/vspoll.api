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
        public async Task<ActionResult<Poll>> Get(int id)
            //ToDo: use possible authenticated user as an argument
            => await pollService.GetPollAsync(id);

        //POST poll [poll data, user]
        //DELETE poll [poll, user]
        //POST poll/vote [bool, user, option]
        //POST poll/block [bool, poll, user, user blocked]
        //POST poll/end [poll, user]
        //DELETE poll/option [option, user]
    }
}
