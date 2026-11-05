namespace Monefy.Api.Models.Entities;

/// <summary>
/// Application-side profile for a user. <see cref="Id"/> is the Supabase
/// <c>auth.users.id</c> so the two stay in sync without a second identity store.
/// </summary>
public class AppUser
{
    public Guid Id { get; set; }

    public string Email { get; set; } = string.Empty;

    public string? FullName { get; set; }

    public string? AvatarUrl { get; set; }

    public string Currency { get; set; } = "USD";

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

    public ICollection<Category> Categories { get; set; } = new List<Category>();
    public ICollection<Expense> Expenses { get; set; } = new List<Expense>();
    public ICollection<Income> Incomes { get; set; } = new List<Income>();
    public ICollection<Budget> Budgets { get; set; } = new List<Budget>();
    public ICollection<Goal> Goals { get; set; } = new List<Goal>();
}
