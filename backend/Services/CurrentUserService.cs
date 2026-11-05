using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Monefy.Api.Helpers;
using Monefy.Api.Services.Interfaces;

namespace Monefy.Api.Services;

public class CurrentUserService(IHttpContextAccessor accessor) : ICurrentUserService
{
    public Guid UserId => TryGetUserId()
        ?? throw new UnauthorizedException("The request is not associated with a signed-in user.");

    public string? Email => FindClaim(ClaimTypes.Email, JwtRegisteredClaimNames.Email);

    public bool IsAuthenticated => TryGetUserId() is not null;

    /// <summary>Supabase puts the user's id in the token's <c>sub</c> claim.</summary>
    private Guid? TryGetUserId()
    {
        var subject = FindClaim(ClaimTypes.NameIdentifier, JwtRegisteredClaimNames.Sub);

        return Guid.TryParse(subject, out var id) ? id : null;
    }

    private string? FindClaim(params string[] types)
    {
        var user = accessor.HttpContext?.User;
        if (user is null)
        {
            return null;
        }

        foreach (var type in types)
        {
            var value = user.FindFirst(type)?.Value;
            if (!string.IsNullOrWhiteSpace(value))
            {
                return value;
            }
        }

        return null;
    }
}
