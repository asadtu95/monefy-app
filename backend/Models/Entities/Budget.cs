using Monefy.Api.Models.Enums;

namespace Monefy.Api.Models.Entities;

public class Budget : BaseEntity
{
    public Guid UserId { get; set; }

    public Guid CategoryId { get; set; }

    public decimal Amount { get; set; }

    public BudgetPeriod Period { get; set; } = BudgetPeriod.Monthly;

    public DateOnly StartDate { get; set; }

    public DateOnly EndDate { get; set; }

    public bool IsActive { get; set; } = true;

    public AppUser User { get; set; } = null!;
    public Category Category { get; set; } = null!;
}
