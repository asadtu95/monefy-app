using System.ComponentModel.DataAnnotations;
using Monefy.Api.Models.Enums;

namespace Monefy.Api.DTOs.Categories;

public class CategoryDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Icon { get; init; } = string.Empty;
    public string Color { get; init; } = string.Empty;
    public TransactionType Type { get; init; }
    public bool IsDefault { get; init; }
}

public class CreateCategoryDto
{
    [Required, StringLength(100, MinimumLength = 1)]
    public string Name { get; init; } = string.Empty;

    [Required, StringLength(50)]
    public string Icon { get; init; } = "ellipsis";

    [Required, RegularExpression("^#(?:[0-9a-fA-F]{3}|[0-9a-fA-F]{6})$",
        ErrorMessage = "Color must be a hex value such as #2a78d6.")]
    public string Color { get; init; } = "#2a78d6";

    [Required]
    public TransactionType Type { get; init; }
}

public class UpdateCategoryDto
{
    [Required, StringLength(100, MinimumLength = 1)]
    public string Name { get; init; } = string.Empty;

    [Required, StringLength(50)]
    public string Icon { get; init; } = "ellipsis";

    [Required, RegularExpression("^#(?:[0-9a-fA-F]{3}|[0-9a-fA-F]{6})$",
        ErrorMessage = "Color must be a hex value such as #2a78d6.")]
    public string Color { get; init; } = "#2a78d6";
}
