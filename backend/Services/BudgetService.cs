using Microsoft.EntityFrameworkCore;
using Monefy.Api.Data;
using Monefy.Api.DTOs.Budgets;
using Monefy.Api.Helpers;
using Monefy.Api.Models.Entities;
using Monefy.Api.Models.Enums;
using Monefy.Api.Services.Interfaces;

namespace Monefy.Api.Services;

/// <summary>
/// A budget is a standing monthly limit for one expense category: set it once and
/// it applies to every month until removed. Spending is always measured live from
/// expenses, so nothing has to be kept in sync when an expense changes.
/// </summary>
public class BudgetService(
    AppDbContext db,
    ICurrentUserService currentUser,
    IPeriodService periods) : IBudgetService
{
    private const decimal WarningThreshold = 80m;

    public async Task<BudgetOverviewDto> GetAsync(int? year, int? month, CancellationToken ct = default)
    {
        var range = await periods.GetRangeAsync(year, month, ct);
        var userId = currentUser.UserId;

        var budgets = await db.Budgets
            .AsNoTracking()
            .Include(b => b.Category)
            .Where(b => b.UserId == userId && b.IsActive)
            .ToListAsync(ct);

        var categoryIds = budgets.Select(b => b.CategoryId).ToList();

        var spentByCategory = await db.Expenses
            .AsNoTracking()
            .Where(e => e.UserId == userId
                        && categoryIds.Contains(e.CategoryId)
                        && e.Date >= range.Start && e.Date <= range.End)
            .GroupBy(e => e.CategoryId)
            .Select(g => new { CategoryId = g.Key, Total = g.Sum(e => e.Amount) })
            .ToDictionaryAsync(x => x.CategoryId, x => x.Total, ct);

        var items = budgets
            .Select(b => ToDto(b, spentByCategory.GetValueOrDefault(b.CategoryId)))
            // Closest to (or over) the limit first — those need attention.
            .OrderByDescending(b => b.PercentageUsed)
            .ThenBy(b => b.CategoryName)
            .ToList();

        var totalBudget = items.Sum(b => b.Amount);
        var totalSpent = items.Sum(b => b.Spent);

        var today = PeriodService.Today;
        var daysLeft = today > range.End ? 0
            : today < range.Start ? range.DayCount
            : range.End.DayNumber - today.DayNumber + 1;

        return new BudgetOverviewDto
        {
            Year = range.Year,
            Month = range.Month,
            StartDate = range.Start,
            EndDate = range.End,
            TotalBudget = totalBudget,
            TotalSpent = totalSpent,
            TotalRemaining = totalBudget - totalSpent,
            PercentageUsed = Percent(totalSpent, totalBudget),
            DaysLeft = daysLeft,
            Budgets = items
        };
    }

    public async Task<BudgetDto> CreateAsync(CreateBudgetDto dto, CancellationToken ct = default)
    {
        var userId = currentUser.UserId;
        var category = await FindExpenseCategoryAsync(dto.CategoryId, ct);

        var exists = await db.Budgets.AnyAsync(
            b => b.UserId == userId && b.CategoryId == dto.CategoryId && b.IsActive, ct);

        if (exists)
        {
            throw new ValidationFailedException(
                $"'{category.Name}' already has a budget. Edit that one instead.");
        }

        var range = await periods.GetRangeAsync(null, null, ct);

        var budget = new Budget
        {
            UserId = userId,
            CategoryId = dto.CategoryId,
            Amount = dto.Amount,
            Period = BudgetPeriod.Monthly,
            StartDate = range.Start,
            EndDate = null,
            IsActive = true,
            Category = category
        };

        db.Budgets.Add(budget);
        await db.SaveChangesAsync(ct);

        return ToDto(budget, await SpentThisMonthAsync(budget.CategoryId, ct));
    }

    public async Task<BudgetDto> UpdateAsync(Guid id, UpdateBudgetDto dto, CancellationToken ct = default)
    {
        var budget = await FindOwnedAsync(id, ct);

        budget.Amount = dto.Amount;
        await db.SaveChangesAsync(ct);

        return ToDto(budget, await SpentThisMonthAsync(budget.CategoryId, ct));
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var budget = await FindOwnedAsync(id, ct);

        db.Budgets.Remove(budget);
        await db.SaveChangesAsync(ct);
    }

    private async Task<Budget> FindOwnedAsync(Guid id, CancellationToken ct) =>
        await db.Budgets
            .Include(b => b.Category)
            .FirstOrDefaultAsync(b => b.Id == id && b.UserId == currentUser.UserId, ct)
        ?? throw new NotFoundException($"Budget {id} was not found.");

    private async Task<Category> FindExpenseCategoryAsync(Guid categoryId, CancellationToken ct)
    {
        var userId = currentUser.UserId;

        var category = await db.Categories
                           .FirstOrDefaultAsync(
                               c => c.Id == categoryId && (c.UserId == null || c.UserId == userId), ct)
                       ?? throw new NotFoundException($"Category {categoryId} was not found.");

        if (category.Type != TransactionType.Expense)
        {
            throw new ValidationFailedException("Budgets can only be set on expense categories.");
        }

        return category;
    }

    private async Task<decimal> SpentThisMonthAsync(Guid categoryId, CancellationToken ct)
    {
        var range = await periods.GetRangeAsync(null, null, ct);
        var userId = currentUser.UserId;

        return await db.Expenses
            .Where(e => e.UserId == userId && e.CategoryId == categoryId
                        && e.Date >= range.Start && e.Date <= range.End)
            .SumAsync(e => e.Amount, ct);
    }

    private static BudgetDto ToDto(Budget budget, decimal spent)
    {
        var percentage = Percent(spent, budget.Amount);

        return new BudgetDto
        {
            Id = budget.Id,
            Amount = budget.Amount,
            CategoryId = budget.CategoryId,
            CategoryName = budget.Category.Name,
            CategoryIcon = budget.Category.Icon,
            CategoryColor = budget.Category.Color,
            Spent = spent,
            Remaining = budget.Amount - spent,
            PercentageUsed = percentage,
            Status = percentage > 100m ? "over" : percentage >= WarningThreshold ? "warning" : "ok"
        };
    }

    private static decimal Percent(decimal part, decimal whole) =>
        whole == 0 ? 0 : Math.Round(part / whole * 100m, 1);
}
