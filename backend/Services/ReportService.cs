using Microsoft.EntityFrameworkCore;
using Monefy.Api.Data;
using Monefy.Api.Data.Seed;
using Monefy.Api.DTOs.Reports;
using Monefy.Api.Helpers;
using Monefy.Api.Services.Interfaces;

namespace Monefy.Api.Services;

public class ReportService(
    AppDbContext db,
    ICurrentUserService currentUser,
    IPeriodService periods) : IReportService
{
    private const int TopCategoryCount = 8;

    public async Task<MonthlyReportDto> GetMonthlyAsync(int? year, int? month, CancellationToken ct = default)
    {
        var current = await periods.GetRangeAsync(year, month, ct);
        var previous = current.Previous();

        // One query spanning both months, split in memory.
        var expenses = await LoadExpensesAsync(previous.Start, current.End, ct);
        var incomes = await LoadIncomesAsync(previous.Start, current.End, ct);

        var currentExpenses = expenses.Where(e => current.Contains(e.Date)).ToList();
        var previousExpenses = expenses.Where(e => previous.Contains(e.Date)).ToList();

        var categories = currentExpenses.Concat(previousExpenses)
            .GroupBy(e => new { e.CategoryId, e.Name, e.Icon, e.Color })
            .Select(g =>
            {
                var now = g.Where(e => current.Contains(e.Date)).Sum(e => e.Amount);
                var before = g.Where(e => previous.Contains(e.Date)).Sum(e => e.Amount);

                return new CategoryComparisonDto
                {
                    CategoryId = g.Key.CategoryId,
                    CategoryName = g.Key.Name,
                    Icon = g.Key.Icon,
                    Color = g.Key.Color,
                    Current = now,
                    Previous = before,
                    Change = now - before,
                    ChangePercent = before == 0 ? null : Math.Round((now - before) / before * 100m, 1)
                };
            })
            .OrderByDescending(c => c.Current)
            .ThenByDescending(c => c.Previous)
            .ToList();

        var largest = currentExpenses.MaxBy(e => e.Amount);

        // Average over the days elapsed so far, so a half-finished month isn't understated.
        var today = PeriodService.Today;
        var elapsedDays = today < current.Start ? 0
            : today > current.End ? current.DayCount
            : today.DayNumber - current.Start.DayNumber + 1;

        var currentTotals = Totals(current, currentExpenses, incomes);

        return new MonthlyReportDto
        {
            Current = currentTotals,
            Previous = Totals(previous, previousExpenses, incomes),
            ExpenseCategories = categories,
            AverageDailyExpense = elapsedDays == 0 ? 0 : Math.Round(currentTotals.Expense / elapsedDays, 2),
            LargestExpense = largest is null
                ? null
                : new LargestExpenseDto
                {
                    Amount = largest.Amount,
                    Date = largest.Date,
                    CategoryName = largest.Name,
                    Note = largest.Note
                }
        };
    }

    public async Task<YearlyReportDto> GetYearlyAsync(int? year, CancellationToken ct = default)
    {
        var startDay = await periods.GetStartDayAsync(ct);
        var targetYear = year ?? MonthRange.Containing(PeriodService.Today, startDay).Year;

        var ranges = Enumerable.Range(1, 12)
            .Select(m => MonthRange.Of(targetYear, m, startDay))
            .ToList();

        var expenses = await LoadExpensesAsync(ranges[0].Start, ranges[^1].End, ct);
        var incomes = await LoadIncomesAsync(ranges[0].Start, ranges[^1].End, ct);

        var months = ranges
            .Select(r => Totals(r, expenses.Where(e => r.Contains(e.Date)).ToList(), incomes))
            .ToList();

        var totalIncome = months.Sum(m => m.Income);
        var totalExpense = months.Sum(m => m.Expense);
        var activeMonths = months.Count(m => m.Income > 0 || m.Expense > 0);

        var topCategories = expenses
            .GroupBy(e => new { e.CategoryId, e.Name, e.Icon, e.Color })
            .Select(g => new CategoryBreakdownDto
            {
                CategoryId = g.Key.CategoryId,
                CategoryName = g.Key.Name,
                Icon = g.Key.Icon,
                Color = g.Key.Color,
                Total = g.Sum(e => e.Amount),
                Percentage = Percent(g.Sum(e => e.Amount), totalExpense),
                TransactionCount = g.Count()
            })
            .OrderByDescending(c => c.Total)
            .Take(TopCategoryCount)
            .ToList();

        return new YearlyReportDto
        {
            Year = targetYear,
            Months = months,
            TotalIncome = totalIncome,
            TotalExpense = totalExpense,
            TotalNet = totalIncome - totalExpense,
            TotalSavings = months.Sum(m => m.Savings),
            SavingsRate = Percent(totalIncome - totalExpense, totalIncome),
            AverageMonthlyIncome = activeMonths == 0 ? 0 : Math.Round(totalIncome / activeMonths, 2),
            AverageMonthlyExpense = activeMonths == 0 ? 0 : Math.Round(totalExpense / activeMonths, 2),
            TopExpenseCategories = topCategories
        };
    }

    private static MonthTotalsDto Totals(
        MonthRange range, IReadOnlyCollection<ExpenseRow> expenses, IEnumerable<IncomeRow> allIncomes)
    {
        var incomes = allIncomes.Where(i => range.Contains(i.Date)).ToList();
        var income = incomes.Sum(i => i.Amount);
        var expense = expenses.Sum(e => e.Amount);

        return new MonthTotalsDto
        {
            Year = range.Year,
            Month = range.Month,
            StartDate = range.Start,
            EndDate = range.End,
            Income = income,
            Expense = expense,
            Net = income - expense,
            Savings = incomes.Where(i => i.CategoryId == SeedData.SavingsCategoryId).Sum(i => i.Amount),
            SavingsRate = Percent(income - expense, income)
        };
    }

    private async Task<List<ExpenseRow>> LoadExpensesAsync(DateOnly from, DateOnly to, CancellationToken ct)
    {
        var userId = currentUser.UserId;

        return await db.Expenses
            .AsNoTracking()
            .Where(e => e.UserId == userId && e.Date >= from && e.Date <= to)
            .Select(e => new ExpenseRow(
                e.Amount, e.Date, e.CategoryId, e.Category.Name, e.Category.Icon, e.Category.Color, e.Note))
            .ToListAsync(ct);
    }

    private async Task<List<IncomeRow>> LoadIncomesAsync(DateOnly from, DateOnly to, CancellationToken ct)
    {
        var userId = currentUser.UserId;

        return await db.Incomes
            .AsNoTracking()
            .Where(i => i.UserId == userId && i.Date >= from && i.Date <= to)
            .Select(i => new IncomeRow(i.Amount, i.Date, i.CategoryId))
            .ToListAsync(ct);
    }

    private static decimal Percent(decimal part, decimal whole) =>
        whole == 0 ? 0 : Math.Round(part / whole * 100m, 1);

    private sealed record ExpenseRow(
        decimal Amount, DateOnly Date, Guid CategoryId, string Name, string Icon, string Color, string? Note);

    private sealed record IncomeRow(decimal Amount, DateOnly Date, Guid CategoryId);
}
