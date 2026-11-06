using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Monefy.Api.DTOs.Common;
using Monefy.Api.DTOs.Reports;
using Monefy.Api.Services.Interfaces;

namespace Monefy.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class ReportsController(IReportService service) : ControllerBase
{
    /// <summary>One month compared with the month before. Defaults to the month today falls in.</summary>
    [HttpGet("monthly")]
    public async Task<ActionResult<ApiResponse<MonthlyReportDto>>> Monthly(
        [FromQuery] int? year, [FromQuery] int? month, CancellationToken ct)
    {
        var report = await service.GetMonthlyAsync(year, month, ct);
        return Ok(ApiResponse<MonthlyReportDto>.Ok(report));
    }

    /// <summary>Twelve months of totals for one year. Defaults to the current year.</summary>
    [HttpGet("yearly")]
    public async Task<ActionResult<ApiResponse<YearlyReportDto>>> Yearly(
        [FromQuery] int? year, CancellationToken ct)
    {
        var report = await service.GetYearlyAsync(year, ct);
        return Ok(ApiResponse<YearlyReportDto>.Ok(report));
    }
}
