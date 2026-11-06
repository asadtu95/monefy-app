using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Monefy.Api.DTOs.Common;
using Monefy.Api.DTOs.Incomes;
using Monefy.Api.Services.Interfaces;

namespace Monefy.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class IncomesController(IIncomeService service) : ControllerBase
{
    /// <summary>Income for one budgeting month. Defaults to the month today falls in.</summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<IncomeDto>>>> Get(
        [FromQuery] int? year, [FromQuery] int? month, CancellationToken ct)
    {
        var incomes = await service.GetAsync(year, month, ct);

        return Ok(ApiResponse<IReadOnlyList<IncomeDto>>.Ok(incomes));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResponse<IncomeDto>>> GetById(Guid id, CancellationToken ct)
    {
        var income = await service.GetByIdAsync(id, ct);
        return Ok(ApiResponse<IncomeDto>.Ok(income));
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<IncomeDto>>> Create(
        [FromBody] CreateIncomeDto dto, CancellationToken ct)
    {
        var income = await service.CreateAsync(dto, ct);
        return CreatedAtAction(nameof(GetById), new { id = income.Id },
            ApiResponse<IncomeDto>.Ok(income, "Income added."));
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ApiResponse<IncomeDto>>> Update(
        Guid id, [FromBody] UpdateIncomeDto dto, CancellationToken ct)
    {
        var income = await service.UpdateAsync(id, dto, ct);
        return Ok(ApiResponse<IncomeDto>.Ok(income, "Income updated."));
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<ApiResponse<object>>> Delete(Guid id, CancellationToken ct)
    {
        await service.DeleteAsync(id, ct);
        return Ok(ApiResponse<object>.Ok(null!, "Income deleted."));
    }
}
