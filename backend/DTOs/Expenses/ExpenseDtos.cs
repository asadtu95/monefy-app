using System.ComponentModel.DataAnnotations;
using Monefy.Api.Models.Enums;

namespace Monefy.Api.DTOs.Expenses;

public class ExpenseDto
{
    public Guid Id { get; init; }
    public decimal Amount { get; init; }
    public DateOnly Date { get; init; }
    public string? Note { get; init; }
    public PaymentMethod PaymentMethod { get; init; }
    public Guid CategoryId { get; init; }
    public string CategoryName { get; init; } = string.Empty;
    public string CategoryIcon { get; init; } = string.Empty;
    public string CategoryColor { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
}

public class CreateExpenseDto
{
    [Range(0.01, 999_999_999, ErrorMessage = "Amount must be greater than zero.")]
    public decimal Amount { get; init; }

    [Required]
    public DateOnly Date { get; init; }

    [StringLength(500)]
    public string? Note { get; init; }

    public PaymentMethod PaymentMethod { get; init; } = PaymentMethod.Cash;

    [Required]
    public Guid CategoryId { get; init; }
}

public class UpdateExpenseDto : CreateExpenseDto;
