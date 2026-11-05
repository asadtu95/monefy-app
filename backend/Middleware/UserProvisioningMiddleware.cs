using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.Extensions.Caching.Memory;
using Monefy.Api.Services;
using Monefy.Api.Services.Interfaces;

namespace Monefy.Api.Middleware;

/// <summary>
/// Guarantees a profile row exists for every authenticated caller. Runs after
/// authentication and caches the ids it has already seen, so the extra query
/// happens once per user per process rather than on every request.
/// </summary>
public class UserProvisioningMiddleware(RequestDelegate next, IMemoryCache cache)
{
    public async Task InvokeAsync(HttpContext context, IUserProvisioningService provisioning)
    {
        var principal = context.User;

        if (principal.Identity?.IsAuthenticated == true)
        {
            var subject = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value
                          ?? principal.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;

            if (Guid.TryParse(subject, out var userId)
                && !cache.TryGetValue(CacheKey(userId), out _))
            {
                var email = principal.FindFirst(ClaimTypes.Email)?.Value
                            ?? principal.FindFirst(JwtRegisteredClaimNames.Email)?.Value;

                var fullName = UserProvisioningService.ReadFullName(
                    principal.FindFirst("user_metadata")?.Value);

                await provisioning.EnsureProfileAsync(userId, email, fullName,
                    context.RequestAborted);

                cache.Set(CacheKey(userId), true, TimeSpan.FromHours(12));
            }
        }

        await next(context);
    }

    private static string CacheKey(Guid userId) => $"profile-provisioned:{userId}";
}
