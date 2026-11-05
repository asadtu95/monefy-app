using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace Monefy.Api.Extensions;

public static class AuthenticationExtensions
{
    /// <summary>
    /// Validates the JWTs Supabase issues. The project signs with ES256 and
    /// publishes the public key at {issuer}/.well-known/jwks.json, so the API
    /// holds no secret — the keys are fetched and cached from the Authority.
    /// </summary>
    public static IServiceCollection AddSupabaseAuth(
        this IServiceCollection services, IConfiguration configuration)
    {
        var issuer = configuration["Supabase:Issuer"]
            ?? throw new InvalidOperationException("Supabase:Issuer is not configured.");

        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.Authority = issuer;
                options.RequireHttpsMetadata = true;

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = issuer,

                    // Supabase stamps every signed-in user token with this audience.
                    ValidateAudience = true,
                    ValidAudience = "authenticated",

                    ValidateIssuerSigningKey = true,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.FromSeconds(30),

                    NameClaimType = ClaimTypes.NameIdentifier,
                };
            });

        services.AddAuthorization();

        return services;
    }
}
