namespace Monefy.Api.Services.Interfaces;

/// <summary>
/// Resolves the owner of the current request from the validated Supabase JWT.
/// </summary>
public interface ICurrentUserService
{
    /// <summary>Throws when the request has no signed-in user.</summary>
    Guid UserId { get; }

    string? Email { get; }

    bool IsAuthenticated { get; }
}
