using Monefy.Api.Models.Entities;

namespace Monefy.Api.Services.Interfaces;

public interface IUserProvisioningService
{
    Task<AppUser> EnsureProfileAsync(
        Guid userId, string? email, string? fullName, CancellationToken ct = default);
}
