using Monefy.Api.Models.Entities;
using Monefy.Api.Models.Enums;

namespace Monefy.Api.Data.Seed;

/// <summary>
/// The built-in categories every user starts with. They carry a null UserId and
/// fixed Ids so re-running the seed is idempotent.
/// </summary>
public static class SeedData
{
    /// <summary>
    /// The dashboard's "Monthly Savings" figure keys off this id rather than the
    /// name, so a user-created category also called "Savings" can't skew it.
    /// </summary>
    public static readonly Guid SavingsCategoryId =
        Guid.Parse("a1000000-0000-0000-0000-000000000003");

    public static IReadOnlyList<Category> DefaultCategories { get; } = new List<Category>
    {
        // ---- Income ----
        Income("a1000000-0000-0000-0000-000000000001", "Salary", "wallet", "#008300"),
        Income("a1000000-0000-0000-0000-000000000002", "Committee", "users", "#1baf7a"),
        Income("a1000000-0000-0000-0000-000000000003", "Savings", "piggy-bank", "#2a78d6"),

        // ---- Expense ----
        Expense("b1000000-0000-0000-0000-000000000001", "Transport", "bus", "#2a78d6"),
        Expense("b1000000-0000-0000-0000-000000000002", "Medical", "heart-pulse", "#e34948"),
        Expense("b1000000-0000-0000-0000-000000000003", "Bills", "receipt", "#eb6834"),
        Expense("b1000000-0000-0000-0000-000000000004", "Home", "house", "#4a3aa7"),
        Expense("b1000000-0000-0000-0000-000000000005", "Food", "utensils", "#eda100"),
        Expense("b1000000-0000-0000-0000-000000000006", "Shopping", "shopping-bag", "#e87ba4"),
        Expense("b1000000-0000-0000-0000-000000000007", "Entertainment", "film", "#1baf7a"),
        Expense("b1000000-0000-0000-0000-000000000008", "Education", "graduation-cap", "#256abf"),
        Expense("b1000000-0000-0000-0000-000000000009", "Mobile/Internet", "wifi", "#008300"),
        Expense("b1000000-0000-0000-0000-00000000000a", "Investment", "trending-up", "#184f95"),
        Expense("b1000000-0000-0000-0000-00000000000b", "Others", "ellipsis", "#898781"),
    };

    private static Category Income(string id, string name, string icon, string color) =>
        Build(id, name, icon, color, TransactionType.Income);

    private static Category Expense(string id, string name, string icon, string color) =>
        Build(id, name, icon, color, TransactionType.Expense);

    private static Category Build(
        string id, string name, string icon, string color, TransactionType type) => new()
    {
        Id = Guid.Parse(id),
        UserId = null,
        Name = name,
        Icon = icon,
        Color = color,
        Type = type,
        IsDefault = true,
        CreatedAt = new DateTime(2026, 8, 1, 0, 0, 0, DateTimeKind.Utc)
    };
}
