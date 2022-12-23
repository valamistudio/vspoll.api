using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VSPoll.API.Models;
using VSPoll.API.Models.Input;
using VSPoll.API.Models.Output;
using VSPoll.API.Services;

namespace VSPoll.API.Controllers;

[Route("option")]
[ApiController]
#pragma warning disable IDE0079
[SuppressMessage("ReSharper", "ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract")]
#pragma warning restore IDE0079
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
    [HttpGet("voters")]
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
        if (!poll!.ShowVoters)
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
    public async Task<ActionResult<PollOption>> Post([FromBody] PollOptionCreate optionCreate)
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

        var poll = await pollService.GetPollAsync(optionCreate.Poll);
        if (poll is null)
            return NotFound("Poll doesn't exist");

        if (!poll.AllowAdd)
            return Conflict("This poll doesn't allow creating new options");

        if (await optionService.CheckDuplicateAsync(optionCreate))
            return Conflict("Option already exists");

        return Ok(await optionService.InsertOptionAsync(optionCreate));
    }

    /// <summary>
    /// Cast a vote on an option
    /// </summary>
    /// <param name="id">The option id</param>
    /// <param name="authentication">The authentication data</param>
    [HttpPost("{id:guid}/vote")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(string), StatusCodes.Status409Conflict)]
    public async Task<ActionResult> Vote(Guid id, [FromBody] Authentication authentication)
    {
        if (authentication is null)
            return BadRequest("Missing authentication payload");

        if (!ModelState.IsValid)
            return BadRequest("Invalid payload");

        if (!userService.Authenticate(authentication, out var error))
            return Unauthorized(error);

        if (!await optionService.CheckIfOptionExistsAsync(id))
            return NotFound("Option doesn't exist");

        var poll = await optionService.GetPollFromOptionAsync(id);
        if (poll!.EndDate < DateTime.UtcNow)
            return Conflict("This poll has expired");

        if (poll.VotingSystem == VotingSystem.Ranked)
            return BadRequest("This poll uses a ranked voting system, call POST poll/{id}/vote");

        var status = await optionService.GetVoteStatusAsync(id, authentication.Id);
        if (status)
            return Ok();

        await userService.AddOrUpdateUserAsync(authentication);

        if (poll.VotingSystem == VotingSystem.SingleOption)
            await optionService.ClearVoteAsync(poll.Id, authentication.Id);

        await optionService.VoteAsync(id, authentication.Id);
        return Ok();
    }

    /// <summary>
    /// Uncast a vote on an option
    /// </summary>
    /// <param name="id">The option id</param>
    /// <param name="authentication">The authentication data</param>
    [HttpDelete("{id:guid}/vote")]
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

        if (!await optionService.CheckIfOptionExistsAsync(id))
            return NotFound("Option doesn't exist");

        var poll = await optionService.GetPollFromOptionAsync(id);
        if (poll!.EndDate < DateTime.UtcNow)
            return Conflict("This poll has expired");

        if (poll.VotingSystem == VotingSystem.Ranked)
            return BadRequest("This poll uses a ranked voting system, call POST poll/{id}/unvote");

        var status = await optionService.GetVoteStatusAsync(id, authentication.Id);
        if (!status)
            return Ok();

        await optionService.UnvoteAsync(id, authentication.Id);
        return Ok();
    }
}
