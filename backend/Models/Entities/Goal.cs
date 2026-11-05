using Monefy.Api.Models.Enums;

namespace Monefy.Api.Models.Entities;

public class Goal : BaseEntity
{
    public Guid UserId { get; set; }

    public string Name { get; set; } = string.Empty;

    public decimal TargetAmount { get; set; }

    public decimal CurrentAmount { get; set; }

    public DateOnly TargetDate { get; set; }

    public GoalStatus Status { get; set; } = GoalStatus.Active;

    public string? Icon { get; set; }

    public AppUser User { get; set; } = null!;
    public ICollection<GoalContribution> Contributions { get; set; } = new List<GoalContribution>();
}
