namespace Monefy.Api.Models.Entities;

public class Income : BaseEntity
{
    public Guid UserId { get; set; }

    public Guid CategoryId { get; set; }

    public decimal Amount { get; set; }

    public DateOnly Date { get; set; }

    public string? Note { get; set; }

    public AppUser User { get; set; } = null!;
    public Category Category { get; set; } = null!;
}
