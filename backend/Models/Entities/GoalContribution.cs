namespace Monefy.Api.Models.Entities;

public class GoalContribution : BaseEntity
{
    public Guid GoalId { get; set; }

    public decimal Amount { get; set; }

    public DateOnly Date { get; set; }

    public string? Note { get; set; }

    public Goal Goal { get; set; } = null!;
}
