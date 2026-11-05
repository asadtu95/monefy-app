using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Monefy.Api.Data;
using Monefy.Api.Models.Entities;
using Monefy.Api.Services.Interfaces;

namespace Monefy.Api.Services;

/// <summary>
/// Supabase owns identity; this table owns the profile. The first time a token
/// arrives for an id we have never seen, create the matching row — without it
/// the first expense would fail on a foreign key.
/// </summary>
public class UserProvisioningService(AppDbContext db) : IUserProvisioningService
{
    public async Task<AppUser> EnsureProfileAsync(
        Guid userId, string? email, string? fullName, CancellationToken ct = default)
    {
        var existing = await db.Users.FirstOrDefaultAsync(u => u.Id == userId, ct);
        if (existing is not null)
        {
            return existing;
        }

        var user = new AppUser
        {
            Id = userId,
            Email = email ?? $"{userId}@unknown.local",
            FullName = fullName,
            Currency = "PKR"
        };

        db.Users.Add(user);

        try
        {
            await db.SaveChangesAsync(ct);
        }
        catch (DbUpdateException)
        {
            // Two concurrent first-requests can race; whichever lost re-reads.
            db.Entry(user).State = EntityState.Detached;

            return await db.Users.FirstAsync(u => u.Id == userId, ct);
        }

        return user;
    }

    /// <summary>
    /// Pulls a display name out of the token's <c>user_metadata</c> claim, which
    /// Supabase populates from whatever was passed at sign-up.
    /// </summary>
    public static string? ReadFullName(string? userMetadataClaim)
    {
        if (string.IsNullOrWhiteSpace(userMetadataClaim))
        {
            return null;
        }

        try
        {
            using var document = JsonDocument.Parse(userMetadataClaim);

            foreach (var key in new[] { "full_name", "name", "fullName" })
            {
                if (document.RootElement.TryGetProperty(key, out var value)
                    && value.ValueKind == JsonValueKind.String)
                {
                    return value.GetString();
                }
            }
        }
        catch (JsonException)
        {
            // Malformed metadata is not worth failing a request over.
        }

        return null;
    }
}
