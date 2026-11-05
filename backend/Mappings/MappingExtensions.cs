using Monefy.Api.DTOs.Categories;
using Monefy.Api.DTOs.Expenses;
using Monefy.Api.DTOs.Incomes;
using Monefy.Api.Models.Entities;

namespace Monefy.Api.Mappings;

/// <summary>
/// Hand-written projections. Kept as expression-bodied statics so they can be
/// used inside LINQ-to-Entities queries without pulling rows into memory.
/// </summary>
public static class MappingExtensions
{
    public static CategoryDto ToDto(this Category category) => new()
    {
        Id = category.Id,
        Name = category.Name,
        Icon = category.Icon,
        Color = category.Color,
        Type = category.Type,
        IsDefault = category.IsDefault
    };

    public static ExpenseDto ToDto(this Expense expense) => new()
    {
        Id = expense.Id,
        Amount = expense.Amount,
        Date = expense.Date,
        Note = expense.Note,
        PaymentMethod = expense.PaymentMethod,
        CategoryId = expense.CategoryId,
        CategoryName = expense.Category.Name,
        CategoryIcon = expense.Category.Icon,
        CategoryColor = expense.Category.Color,
        CreatedAt = expense.CreatedAt
    };

    public static IncomeDto ToDto(this Income income) => new()
    {
        Id = income.Id,
        Amount = income.Amount,
        Date = income.Date,
        Note = income.Note,
        CategoryId = income.CategoryId,
        CategoryName = income.Category.Name,
        CategoryIcon = income.Category.Icon,
        CategoryColor = income.Category.Color,
        CreatedAt = income.CreatedAt
    };
}
