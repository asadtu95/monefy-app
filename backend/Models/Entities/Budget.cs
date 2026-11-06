using Monefy.Api.Models.Enums;

namespace Monefy.Api.Models.Entities;

public class Budget : BaseEntity
{
    public Guid UserId { get; set; }

    public Guid CategoryId { get; set; }

    public decimal Amount { get; set; }

    public BudgetPeriod Period { get; set; } = BudgetPeriod.Monthly;

    /// <summary>First day the limit applies from.</summary>
    public DateOnly StartDate { get; set; }

    /// <summary>Null while the limit repeats every month with no end.</summary>
    public DateOnly? EndDate { get; set; }

    public bool IsActive { get; set; } = true;

    public AppUser User { get; set; } = null!;
    public Category Category { get; set; } = null!;
}
