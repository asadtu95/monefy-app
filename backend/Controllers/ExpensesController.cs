using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Monefy.Api.DTOs.Common;
using Monefy.Api.DTOs.Expenses;
using Monefy.Api.Services.Interfaces;

namespace Monefy.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class ExpensesController(IExpenseService service) : ControllerBase
{
    /// <summary>Expenses for one calendar month. Defaults to the current month.</summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<ExpenseDto>>>> Get(
        [FromQuery] int? year, [FromQuery] int? month, CancellationToken ct)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var expenses = await service.GetAsync(year ?? today.Year, month ?? today.Month, ct);

        return Ok(ApiResponse<IReadOnlyList<ExpenseDto>>.Ok(expenses));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResponse<ExpenseDto>>> GetById(Guid id, CancellationToken ct)
    {
        var expense = await service.GetByIdAsync(id, ct);
        return Ok(ApiResponse<ExpenseDto>.Ok(expense));
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<ExpenseDto>>> Create(
        [FromBody] CreateExpenseDto dto, CancellationToken ct)
    {
        var expense = await service.CreateAsync(dto, ct);
        return CreatedAtAction(nameof(GetById), new { id = expense.Id },
            ApiResponse<ExpenseDto>.Ok(expense, "Expense added."));
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ApiResponse<ExpenseDto>>> Update(
        Guid id, [FromBody] UpdateExpenseDto dto, CancellationToken ct)
    {
        var expense = await service.UpdateAsync(id, dto, ct);
        return Ok(ApiResponse<ExpenseDto>.Ok(expense, "Expense updated."));
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<ApiResponse<object>>> Delete(Guid id, CancellationToken ct)
    {
        await service.DeleteAsync(id, ct);
        return Ok(ApiResponse<object>.Ok(null!, "Expense deleted."));
    }
}
