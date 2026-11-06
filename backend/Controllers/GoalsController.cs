using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Monefy.Api.DTOs.Common;
using Monefy.Api.DTOs.Goals;
using Monefy.Api.Services.Interfaces;

namespace Monefy.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class GoalsController(IGoalService service) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<GoalDto>>>> GetAll(CancellationToken ct)
    {
        var goals = await service.GetAllAsync(ct);
        return Ok(ApiResponse<IReadOnlyList<GoalDto>>.Ok(goals));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResponse<GoalDto>>> GetById(Guid id, CancellationToken ct)
    {
        var goal = await service.GetByIdAsync(id, ct);
        return Ok(ApiResponse<GoalDto>.Ok(goal));
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<GoalDto>>> Create(
        [FromBody] CreateGoalDto dto, CancellationToken ct)
    {
        var goal = await service.CreateAsync(dto, ct);
        return CreatedAtAction(nameof(GetById), new { id = goal.Id },
            ApiResponse<GoalDto>.Ok(goal, "Goal created."));
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ApiResponse<GoalDto>>> Update(
        Guid id, [FromBody] UpdateGoalDto dto, CancellationToken ct)
    {
        var goal = await service.UpdateAsync(id, dto, ct);
        return Ok(ApiResponse<GoalDto>.Ok(goal, "Goal updated."));
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<ApiResponse<object>>> Delete(Guid id, CancellationToken ct)
    {
        await service.DeleteAsync(id, ct);
        return Ok(ApiResponse<object>.Ok(null!, "Goal deleted."));
    }

    /// <summary>Adds money to (or, with a negative amount, takes it out of) a goal.</summary>
    [HttpPost("{id:guid}/contributions")]
    public async Task<ActionResult<ApiResponse<GoalDto>>> AddContribution(
        Guid id, [FromBody] CreateContributionDto dto, CancellationToken ct)
    {
        var goal = await service.AddContributionAsync(id, dto, ct);
        return Ok(ApiResponse<GoalDto>.Ok(goal, "Saved."));
    }

    [HttpDelete("{id:guid}/contributions/{contributionId:guid}")]
    public async Task<ActionResult<ApiResponse<GoalDto>>> DeleteContribution(
        Guid id, Guid contributionId, CancellationToken ct)
    {
        var goal = await service.DeleteContributionAsync(id, contributionId, ct);
        return Ok(ApiResponse<GoalDto>.Ok(goal, "Entry removed."));
    }
}
