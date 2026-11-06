using System.ComponentModel.DataAnnotations;

namespace Monefy.Api.DTOs.Budgets;

/// <summary>One monthly spending limit, measured against a single month.</summary>
public class BudgetDto
{
    public Guid Id { get; init; }
    public decimal Amount { get; init; }
    public Guid CategoryId { get; init; }
    public string CategoryName { get; init; } = string.Empty;
    public string CategoryIcon { get; init; } = string.Empty;
    public string CategoryColor { get; init; } = string.Empty;

    /// <summary>Expenses booked to the category inside the month.</summary>
    public decimal Spent { get; init; }

    /// <summary>Amount - Spent. Negative once the limit is exceeded.</summary>
    public decimal Remaining { get; init; }

    /// <summary>Spent as a share of Amount. Can exceed 100.</summary>
    public decimal PercentageUsed { get; init; }

    /// <summary>"ok" below 80%, "warning" from 80%, "over" above 100%.</summary>
    public string Status { get; init; } = "ok";
}

public class BudgetOverviewDto
{
    public int Year { get; init; }
    public int Month { get; init; }
    public DateOnly StartDate { get; init; }
    public DateOnly EndDate { get; init; }

    public decimal TotalBudget { get; init; }
    public decimal TotalSpent { get; init; }
    public decimal TotalRemaining { get; init; }
    public decimal PercentageUsed { get; init; }

    /// <summary>Days left in the month including today; 0 for past months.</summary>
    public int DaysLeft { get; init; }

    public IReadOnlyList<BudgetDto> Budgets { get; init; } = [];
}

public class CreateBudgetDto
{
    [Required]
    public Guid CategoryId { get; init; }

    [Range(1, 999_999_999, ErrorMessage = "Limit must be at least 1.")]
    public decimal Amount { get; init; }
}

public class UpdateBudgetDto
{
    [Range(1, 999_999_999, ErrorMessage = "Limit must be at least 1.")]
    public decimal Amount { get; init; }
}
