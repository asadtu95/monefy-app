using Monefy.Api.DTOs.Categories;
using Monefy.Api.DTOs.Expenses;
using Monefy.Api.DTOs.Incomes;
using Monefy.Api.DTOs.Reports;
using Monefy.Api.Models.Enums;

namespace Monefy.Api.Services.Interfaces;

public interface ICategoryService
{
    Task<IReadOnlyList<CategoryDto>> GetAllAsync(TransactionType? type, CancellationToken ct = default);
    Task<CategoryDto> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<CategoryDto> CreateAsync(CreateCategoryDto dto, CancellationToken ct = default);
    Task<CategoryDto> UpdateAsync(Guid id, UpdateCategoryDto dto, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
}

public interface IExpenseService
{
    Task<IReadOnlyList<ExpenseDto>> GetAsync(int year, int month, CancellationToken ct = default);
    Task<ExpenseDto> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<ExpenseDto> CreateAsync(CreateExpenseDto dto, CancellationToken ct = default);
    Task<ExpenseDto> UpdateAsync(Guid id, UpdateExpenseDto dto, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
}

public interface IIncomeService
{
    Task<IReadOnlyList<IncomeDto>> GetAsync(int year, int month, CancellationToken ct = default);
    Task<IncomeDto> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IncomeDto> CreateAsync(CreateIncomeDto dto, CancellationToken ct = default);
    Task<IncomeDto> UpdateAsync(Guid id, UpdateIncomeDto dto, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
}

public interface IDashboardService
{
    Task<DashboardDto> GetAsync(int year, int month, CancellationToken ct = default);
}
