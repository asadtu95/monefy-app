using System.ComponentModel.DataAnnotations;

namespace Monefy.Api.DTOs.Auth;

public class UserProfileDto
{
    public Guid Id { get; init; }
    public string Email { get; init; } = string.Empty;
    public string? FullName { get; init; }
    public string? AvatarUrl { get; init; }
    public string Currency { get; init; } = "PKR";
    public DateTime CreatedAt { get; init; }
}

public class RegisterDto
{
    [Required, EmailAddress, StringLength(256)]
    public string Email { get; init; } = string.Empty;

    [Required, StringLength(72, MinimumLength = 6,
        ErrorMessage = "Password must be between 6 and 72 characters.")]
    public string Password { get; init; } = string.Empty;

    [StringLength(150)]
    public string? FullName { get; init; }
}

public class RegisterResultDto
{
    public Guid UserId { get; init; }
    public string Email { get; init; } = string.Empty;
}

public class UpdateProfileDto
{
    [StringLength(150)]
    public string? FullName { get; init; }

    [StringLength(3, MinimumLength = 3)]
    public string? Currency { get; init; }
}
