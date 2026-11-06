namespace Monefy.Api.DTOs.Reports;

/// <summary>Income, expense and savings for one budgeting month.</summary>
public class MonthTotalsDto
{
    public int Year { get; init; }
    public int Month { get; init; }
    public DateOnly StartDate { get; init; }
    public DateOnly EndDate { get; init; }
    public decimal Income { get; init; }
    public decimal Expense { get; init; }

    /// <summary>Income - Expense.</summary>
    public decimal Net { get; init; }

    /// <summary>Only income booked to the built-in Savings category.</summary>
    public decimal Savings { get; init; }

    /// <summary>Net as a share of Income, 0 when there was no income.</summary>
    public decimal SavingsRate { get; init; }
}

/// <summary>A category's spend in two months side by side.</summary>
public class CategoryComparisonDto
{
    public Guid CategoryId { get; init; }
    public string CategoryName { get; init; } = string.Empty;
    public string Icon { get; init; } = string.Empty;
    public string Color { get; init; } = string.Empty;
    public decimal Current { get; init; }
    public decimal Previous { get; init; }
    public decimal Change { get; init; }

    /// <summary>Null when the category had no spend last month.</summary>
    public decimal? ChangePercent { get; init; }
}

public class MonthlyReportDto
{
    public MonthTotalsDto Current { get; init; } = new();
    public MonthTotalsDto Previous { get; init; } = new();

    /// <summary>Expense categories ordered by this month's spend, then last month's.</summary>
    public IReadOnlyList<CategoryComparisonDto> ExpenseCategories { get; init; } = [];

    public decimal AverageDailyExpense { get; init; }

    /// <summary>The single largest expense of the month, if any.</summary>
    public LargestExpenseDto? LargestExpense { get; init; }
}

public class LargestExpenseDto
{
    public decimal Amount { get; init; }
    public DateOnly Date { get; init; }
    public string CategoryName { get; init; } = string.Empty;
    public string? Note { get; init; }
}

public class YearlyReportDto
{
    public int Year { get; init; }

    /// <summary>Always twelve entries, January first.</summary>
    public IReadOnlyList<MonthTotalsDto> Months { get; init; } = [];

    public decimal TotalIncome { get; init; }
    public decimal TotalExpense { get; init; }
    public decimal TotalNet { get; init; }
    public decimal TotalSavings { get; init; }
    public decimal SavingsRate { get; init; }

    /// <summary>Averages over months that had any activity.</summary>
    public decimal AverageMonthlyIncome { get; init; }
    public decimal AverageMonthlyExpense { get; init; }

    public IReadOnlyList<CategoryBreakdownDto> TopExpenseCategories { get; init; } = [];
}
