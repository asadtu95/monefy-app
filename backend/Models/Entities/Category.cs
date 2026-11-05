using Monefy.Api.Models.Enums;

namespace Monefy.Api.Models.Entities;

public class Category : BaseEntity
{
    /// <summary>Null for the built-in categories shared by every user.</summary>
    public Guid? UserId { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Icon { get; set; } = string.Empty;

    public string Color { get; set; } = "#3b82f6";

    public TransactionType Type { get; set; }

    public bool IsDefault { get; set; }

    public AppUser? User { get; set; }
    public ICollection<Expense> Expenses { get; set; } = new List<Expense>();
    public ICollection<Income> Incomes { get; set; } = new List<Income>();
    public ICollection<Budget> Budgets { get; set; } = new List<Budget>();
}
