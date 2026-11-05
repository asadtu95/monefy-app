using Microsoft.EntityFrameworkCore;

namespace Monefy.Api.Data.Seed;

public static class DbInitializer
{
    /// <summary>
    /// Seeds only the built-in categories. User rows are created on demand from
    /// the Supabase token by <see cref="Services.UserProvisioningService"/> —
    /// there is no placeholder account any more.
    /// </summary>
    public static async Task SeedAsync(AppDbContext db, CancellationToken cancellationToken = default)
    {
        var existingIds = await db.Categories
            .Where(c => c.IsDefault)
            .Select(c => c.Id)
            .ToListAsync(cancellationToken);

        var missing = SeedData.DefaultCategories
            .Where(c => !existingIds.Contains(c.Id))
            .ToList();

        if (missing.Count == 0)
        {
            return;
        }

        db.Categories.AddRange(missing);
        await db.SaveChangesAsync(cancellationToken);
    }
}
