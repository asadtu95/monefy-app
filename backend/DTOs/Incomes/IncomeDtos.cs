using System.ComponentModel.DataAnnotations;

namespace Monefy.Api.DTOs.Incomes;

public class IncomeDto
{
    public Guid Id { get; init; }
    public decimal Amount { get; init; }
    public DateOnly Date { get; init; }
    public string? Note { get; init; }
    public Guid CategoryId { get; init; }
    public string CategoryName { get; init; } = string.Empty;
    public string CategoryIcon { get; init; } = string.Empty;
    public string CategoryColor { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
}

public class CreateIncomeDto
{
    [Range(0.01, 999_999_999, ErrorMessage = "Amount must be greater than zero.")]
    public decimal Amount { get; init; }

    [Required]
    public DateOnly Date { get; init; }

    [StringLength(500)]
    public string? Note { get; init; }

    [Required]
    public Guid CategoryId { get; init; }
}

public class UpdateIncomeDto : CreateIncomeDto;
