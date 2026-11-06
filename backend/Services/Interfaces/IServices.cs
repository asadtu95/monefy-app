using Monefy.Api.DTOs.Budgets;
using Monefy.Api.DTOs.Categories;
using Monefy.Api.DTOs.Expenses;
using Monefy.Api.DTOs.Goals;
using Monefy.Api.DTOs.Incomes;
using Monefy.Api.DTOs.Reports;
using Monefy.Api.Helpers;
using Monefy.Api.Models.Enums;

namespace Monefy.Api.Services.Interfaces;

/// <summary>
/// The caller's budgeting month. A null year/month means "the month today is in".
/// </summary>
public interface IPeriodService
{
    Task<int> GetStartDayAsync(CancellationToken ct = default);
    Task<MonthRange> GetRangeAsync(int? year, int? month, CancellationToken ct = default);
}

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
    Task<IReadOnlyList<ExpenseDto>> GetAsync(int? year, int? month, CancellationToken ct = default);
    Task<ExpenseDto> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<ExpenseDto> CreateAsync(CreateExpenseDto dto, CancellationToken ct = default);
    Task<ExpenseDto> UpdateAsync(Guid id, UpdateExpenseDto dto, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
}

public interface IIncomeService
{
    Task<IReadOnlyList<IncomeDto>> GetAsync(int? year, int? month, CancellationToken ct = default);
    Task<IncomeDto> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IncomeDto> CreateAsync(CreateIncomeDto dto, CancellationToken ct = default);
    Task<IncomeDto> UpdateAsync(Guid id, UpdateIncomeDto dto, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
}

public interface IDashboardService
{
    Task<DashboardDto> GetAsync(int? year, int? month, CancellationToken ct = default);
}

public interface IBudgetService
{
    /// <summary>Every active limit, with spending measured inside the given month.</summary>
    Task<BudgetOverviewDto> GetAsync(int? year, int? month, CancellationToken ct = default);
    Task<BudgetDto> CreateAsync(CreateBudgetDto dto, CancellationToken ct = default);
    Task<BudgetDto> UpdateAsync(Guid id, UpdateBudgetDto dto, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
}

public interface IGoalService
{
    Task<IReadOnlyList<GoalDto>> GetAllAsync(CancellationToken ct = default);
    Task<GoalDto> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<GoalDto> CreateAsync(CreateGoalDto dto, CancellationToken ct = default);
    Task<GoalDto> UpdateAsync(Guid id, UpdateGoalDto dto, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
    Task<GoalDto> AddContributionAsync(Guid goalId, CreateContributionDto dto, CancellationToken ct = default);
    Task<GoalDto> DeleteContributionAsync(Guid goalId, Guid contributionId, CancellationToken ct = default);
}

public interface IReportService
{
    Task<MonthlyReportDto> GetMonthlyAsync(int? year, int? month, CancellationToken ct = default);
    Task<YearlyReportDto> GetYearlyAsync(int? year, CancellationToken ct = default);
}
