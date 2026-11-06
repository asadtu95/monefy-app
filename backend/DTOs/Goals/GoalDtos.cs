using System.ComponentModel.DataAnnotations;
using Monefy.Api.Models.Enums;

namespace Monefy.Api.DTOs.Goals;

public class GoalDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? Icon { get; init; }
    public decimal TargetAmount { get; init; }
    public decimal CurrentAmount { get; init; }
    public decimal RemainingAmount { get; init; }
    public decimal PercentageComplete { get; init; }
    public DateOnly TargetDate { get; init; }
    public GoalStatus Status { get; init; }

    /// <summary>Whole months from today to the target date, at least 1 while active.</summary>
    public int MonthsLeft { get; init; }

    /// <summary>What to put aside each month to finish on time. 0 once reached.</summary>
    public decimal MonthlyNeeded { get; init; }

    /// <summary>Target date has passed and the goal is still short.</summary>
    public bool IsOverdue { get; init; }

    public DateTime CreatedAt { get; init; }

    /// <summary>Newest first.</summary>
    public IReadOnlyList<GoalContributionDto> Contributions { get; init; } = [];
}

public class GoalContributionDto
{
    public Guid Id { get; init; }
    public decimal Amount { get; init; }
    public DateOnly Date { get; init; }
    public string? Note { get; init; }
}

public class CreateGoalDto
{
    [Required, StringLength(150, MinimumLength = 1)]
    public string Name { get; init; } = string.Empty;

    [Range(1, 999_999_999, ErrorMessage = "Target must be at least 1.")]
    public decimal TargetAmount { get; init; }

    [Required]
    public DateOnly TargetDate { get; init; }

    [StringLength(50)]
    public string? Icon { get; init; }
}

public class UpdateGoalDto : CreateGoalDto
{
    /// <summary>Only Active or Cancelled can be set by hand; Achieved follows the balance.</summary>
    public GoalStatus? Status { get; init; }
}

public class CreateContributionDto
{
    /// <summary>Negative to record a withdrawal from the goal.</summary>
    [Range(-999_999_999, 999_999_999)]
    public decimal Amount { get; init; }

    [Required]
    public DateOnly Date { get; init; }

    [StringLength(500)]
    public string? Note { get; init; }
}
