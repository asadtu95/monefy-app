using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Monefy.Api.DTOs.Common;
using Monefy.Api.DTOs.Reports;
using Monefy.Api.Services.Interfaces;

namespace Monefy.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class DashboardController(IDashboardService service) : ControllerBase
{
    /// <summary>Every dashboard figure for one month. Defaults to the current month.</summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<DashboardDto>>> Get(
        [FromQuery] int? year, [FromQuery] int? month, CancellationToken ct)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var dashboard = await service.GetAsync(year ?? today.Year, month ?? today.Month, ct);

        return Ok(ApiResponse<DashboardDto>.Ok(dashboard));
    }
}
