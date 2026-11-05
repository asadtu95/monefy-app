namespace Monefy.Api.Services.Interfaces;

public interface ISupabaseAdminClient
{
    /// <summary>False when no service_role key is configured, so sign-up is disabled.</summary>
    bool IsConfigured { get; }

    /// <summary>
    /// Creates a Supabase user whose email is already confirmed, so they can sign
    /// in immediately without a confirmation link.
    /// </summary>
    Task<Guid> CreateConfirmedUserAsync(
        string email, string password, string? fullName, CancellationToken ct = default);
}
