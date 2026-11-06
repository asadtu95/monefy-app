using Microsoft.EntityFrameworkCore;
using Monefy.Api.Data;
using Monefy.Api.Helpers;
using Monefy.Api.Services.Interfaces;

namespace Monefy.Api.Services;

/// <summary>
/// Turns a (year, month) pair into concrete dates using the caller's month start day.
/// Scoped, so the start day is read from the database at most once per request.
/// </summary>
public class PeriodService(AppDbContext db, ICurrentUserService currentUser) : IPeriodService
{
    private int? startDay;

    public async Task<int> GetStartDayAsync(CancellationToken ct = default)
    {
        if (startDay is { } cached)
        {
            return cached;
        }

        var userId = currentUser.UserId;

        var stored = await db.Users
            .AsNoTracking()
            .Where(u => u.Id == userId)
            .Select(u => (int?)u.MonthStartDay)
            .FirstOrDefaultAsync(ct);

        startDay = Math.Clamp(stored ?? 1, MonthRange.MinStartDay, MonthRange.MaxStartDay);
        return startDay.Value;
    }

    public async Task<MonthRange> GetRangeAsync(int? year, int? month, CancellationToken ct = default)
    {
        var day = await GetStartDayAsync(ct);

        if (year is { } y && month is { } m)
        {
            return MonthRange.Of(y, m, day);
        }

        // No month asked for: whichever budgeting month today falls in.
        return MonthRange.Containing(Today, day);
    }

    public static DateOnly Today => DateOnly.FromDateTime(DateTime.UtcNow);
}
