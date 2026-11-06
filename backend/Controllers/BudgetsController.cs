using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Monefy.Api.DTOs.Budgets;
using Monefy.Api.DTOs.Common;
using Monefy.Api.Services.Interfaces;

namespace Monefy.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class BudgetsController(IBudgetService service) : ControllerBase
{
    /// <summary>Every monthly limit with its spend for one month. Defaults to the month today falls in.</summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<BudgetOverviewDto>>> Get(
        [FromQuery] int? year, [FromQuery] int? month, CancellationToken ct)
    {
        var overview = await service.GetAsync(year, month, ct);
        return Ok(ApiResponse<BudgetOverviewDto>.Ok(overview));
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<BudgetDto>>> Create(
        [FromBody] CreateBudgetDto dto, CancellationToken ct)
    {
        var budget = await service.CreateAsync(dto, ct);
        return Ok(ApiResponse<BudgetDto>.Ok(budget, "Budget saved."));
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ApiResponse<BudgetDto>>> Update(
        Guid id, [FromBody] UpdateBudgetDto dto, CancellationToken ct)
    {
        var budget = await service.UpdateAsync(id, dto, ct);
        return Ok(ApiResponse<BudgetDto>.Ok(budget, "Budget updated."));
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<ApiResponse<object>>> Delete(Guid id, CancellationToken ct)
    {
        await service.DeleteAsync(id, ct);
        return Ok(ApiResponse<object>.Ok(null!, "Budget removed."));
    }
}
