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
    /// <summary>Every dashboard figure for one budgeting month. Defaults to the month today falls in.</summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<DashboardDto>>> Get(
        [FromQuery] int? year, [FromQuery] int? month, CancellationToken ct)
    {
        var dashboard = await service.GetAsync(year, month, ct);

        return Ok(ApiResponse<DashboardDto>.Ok(dashboard));
    }
}
