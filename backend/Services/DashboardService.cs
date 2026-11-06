using Microsoft.EntityFrameworkCore;
using Monefy.Api.Data;
using Monefy.Api.Data.Seed;
using Monefy.Api.DTOs.Reports;
using Monefy.Api.Helpers;
using Monefy.Api.Services.Interfaces;

namespace Monefy.Api.Services;

public class DashboardService(
    AppDbContext db,
    ICurrentUserService currentUser,
    IPeriodService periods) : IDashboardService
{
    public async Task<DashboardDto> GetAsync(
        int? year, int? month, CancellationToken ct = default)
    {
        var range = await periods.GetRangeAsync(year, month, ct);
        var userId = currentUser.UserId;

        var expenses = await db.Expenses
            .AsNoTracking()
            .Where(e => e.UserId == userId && e.Date >= range.Start && e.Date <= range.End)
            .Select(e => new
            {
                e.Amount,
                e.Date,
                e.CategoryId,
                e.Category.Name,
                e.Category.Icon,
                e.Category.Color
            })
            .ToListAsync(ct);

        var incomes = await db.Incomes
            .AsNoTracking()
            .Where(i => i.UserId == userId && i.Date >= range.Start && i.Date <= range.End)
            .Select(i => new
            {
                i.Amount,
                i.Date,
                i.CategoryId,
                i.Category.Name,
                i.Category.Icon,
                i.Category.Color
            })
            .ToListAsync(ct);

        var totalIncome = incomes.Sum(i => i.Amount);
        var totalExpense = expenses.Sum(e => e.Amount);

        var expenseByCategory = expenses
            .GroupBy(e => new { e.CategoryId, e.Name, e.Icon, e.Color })
            .Select(g => new CategoryBreakdownDto
            {
                CategoryId = g.Key.CategoryId,
                CategoryName = g.Key.Name,
                Icon = g.Key.Icon,
                Color = g.Key.Color,
                Total = g.Sum(x => x.Amount),
                Percentage = Percent(g.Sum(x => x.Amount), totalExpense),
                TransactionCount = g.Count()
            })
            .OrderByDescending(c => c.Total)
            .ToList();

        var incomeByCategory = incomes
            .GroupBy(i => new { i.CategoryId, i.Name, i.Icon, i.Color })
            .Select(g => new CategoryBreakdownDto
            {
                CategoryId = g.Key.CategoryId,
                CategoryName = g.Key.Name,
                Icon = g.Key.Icon,
                Color = g.Key.Color,
                Total = g.Sum(x => x.Amount),
                Percentage = Percent(g.Sum(x => x.Amount), totalIncome),
                TransactionCount = g.Count()
            })
            .OrderByDescending(c => c.Total)
            .ToList();

        var expenseByDay = expenses
            .GroupBy(e => e.Date)
            .ToDictionary(g => g.Key, g => g.Sum(x => x.Amount));

        var incomeByDay = incomes
            .GroupBy(i => i.Date)
            .ToDictionary(g => g.Key, g => g.Sum(x => x.Amount));

        // Zero-fill every day so the chart keeps a continuous axis in a sparse month.
        var dailyTotals = Enumerable.Range(0, range.DayCount)
            .Select(offset => range.Start.AddDays(offset))
            .Select(date => new DailyTotalDto
            {
                Date = date,
                Income = incomeByDay.GetValueOrDefault(date),
                Expense = expenseByDay.GetValueOrDefault(date)
            })
            .ToList();

        return new DashboardDto
        {
            Year = range.Year,
            Month = range.Month,
            TotalIncome = totalIncome,
            TotalExpense = totalExpense,
            RemainingBalance = totalIncome - totalExpense,
            MonthlySavings = incomes
                .Where(i => i.CategoryId == SeedData.SavingsCategoryId)
                .Sum(i => i.Amount),
            ExpenseByCategory = expenseByCategory,
            IncomeByCategory = incomeByCategory,
            DailyTotals = dailyTotals,
            TransactionCount = expenses.Count + incomes.Count,
            HasData = expenses.Count > 0 || incomes.Count > 0
        };
    }

    private static decimal Percent(decimal part, decimal whole) =>
        whole == 0 ? 0 : Math.Round(part / whole * 100m, 1);
}
