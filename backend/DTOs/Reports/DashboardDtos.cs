namespace Monefy.Api.DTOs.Reports;

public class CategoryBreakdownDto
{
    public Guid CategoryId { get; init; }
    public string CategoryName { get; init; } = string.Empty;
    public string Icon { get; init; } = string.Empty;
    public string Color { get; init; } = string.Empty;
    public decimal Total { get; init; }
    public decimal Percentage { get; init; }
    public int TransactionCount { get; init; }
}

public class DailyTotalDto
{
    public DateOnly Date { get; init; }
    public decimal Income { get; init; }
    public decimal Expense { get; init; }
}

public class DashboardDto
{
    public int Year { get; init; }
    public int Month { get; init; }

    /// <summary>Every income transaction in the month, Savings included.</summary>
    public decimal TotalIncome { get; init; }

    public decimal TotalExpense { get; init; }

    /// <summary>TotalIncome - TotalExpense.</summary>
    public decimal RemainingBalance { get; init; }

    /// <summary>Only what was booked to the "Savings" income category.</summary>
    public decimal MonthlySavings { get; init; }

    public IReadOnlyList<CategoryBreakdownDto> ExpenseByCategory { get; init; } = [];
    public IReadOnlyList<CategoryBreakdownDto> IncomeByCategory { get; init; } = [];
    public IReadOnlyList<DailyTotalDto> DailyTotals { get; init; } = [];

    public int TransactionCount { get; init; }
    public bool HasData { get; init; }
}
