using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Monefy.Api.Data;
using Monefy.Api.DTOs.Auth;
using Monefy.Api.DTOs.Common;
using Monefy.Api.Helpers;
using Monefy.Api.Services.Interfaces;

namespace Monefy.Api.Controllers;

/// <summary>
/// Sign-up and sign-in happen against Supabase directly; this controller only
/// exposes the application-side profile that hangs off the authenticated id.
/// </summary>
[ApiController]
[Authorize]
[Route("api/[controller]")]
public class AuthController(
    AppDbContext db,
    ICurrentUserService currentUser,
    ISupabaseAdminClient supabaseAdmin) : ControllerBase
{
    /// <summary>
    /// Creates an account that is confirmed on the spot, so the caller can sign
    /// in straight away without a confirmation email. Sign-in itself still goes
    /// to Supabase directly from the browser — this endpoint issues no tokens.
    /// </summary>
    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<ActionResult<ApiResponse<RegisterResultDto>>> Register(
        [FromBody] RegisterDto dto, CancellationToken ct)
    {
        var email = dto.Email.Trim().ToLowerInvariant();

        var userId = await supabaseAdmin.CreateConfirmedUserAsync(
            email, dto.Password, dto.FullName?.Trim(), ct);

        return Ok(ApiResponse<RegisterResultDto>.Ok(
            new RegisterResultDto { UserId = userId, Email = email },
            "Account created."));
    }

    /// <summary>The signed-in user's profile. Created on first call if absent.</summary>
    [HttpGet("me")]
    public async Task<ActionResult<ApiResponse<UserProfileDto>>> Me(CancellationToken ct)
    {
        var user = await db.Users
                       .AsNoTracking()
                       .FirstOrDefaultAsync(u => u.Id == currentUser.UserId, ct)
                   ?? throw new NotFoundException("Profile not found.");

        return Ok(ApiResponse<UserProfileDto>.Ok(ToDto(user)));
    }

    [HttpPut("me")]
    public async Task<ActionResult<ApiResponse<UserProfileDto>>> UpdateMe(
        [FromBody] UpdateProfileDto dto, CancellationToken ct)
    {
        var user = await db.Users.FirstOrDefaultAsync(u => u.Id == currentUser.UserId, ct)
                   ?? throw new NotFoundException("Profile not found.");

        if (dto.FullName is not null)
        {
            user.FullName = dto.FullName.Trim();
        }

        if (!string.IsNullOrWhiteSpace(dto.Currency))
        {
            user.Currency = dto.Currency.Trim().ToUpperInvariant();
        }

        if (dto.MonthStartDay is { } startDay)
        {
            user.MonthStartDay = startDay;
        }

        await db.SaveChangesAsync(ct);

        return Ok(ApiResponse<UserProfileDto>.Ok(ToDto(user), "Profile updated."));
    }

    private static UserProfileDto ToDto(Models.Entities.AppUser user) => new()
    {
        Id = user.Id,
        Email = user.Email,
        FullName = user.FullName,
        AvatarUrl = user.AvatarUrl,
        Currency = user.Currency,
        MonthStartDay = user.MonthStartDay,
        CreatedAt = user.CreatedAt
    };
}
