namespace Monefy.Api.Helpers;

/// <summary>
/// Inclusive first/last day of one budgeting month.
/// </summary>
/// <remarks>
/// With the default start day of 1 this is the calendar month. With a custom start
/// day (e.g. salary on the 25th) the month runs from that day to the day before it
/// in the next month, and is named after the calendar month holding most of its days:
/// <list type="bullet">
///   <item>start day 1–15: "November" = 5 Nov – 4 Dec (starts in the named month)</item>
///   <item>start day 16–28: "November" = 25 Oct – 24 Nov (ends in the named month)</item>
/// </list>
/// </remarks>
public readonly record struct MonthRange(int Year, int Month, DateOnly Start, DateOnly End)
{
    public const int MinStartDay = 1;

    /// <summary>Capped at 28 so every month has the day.</summary>
    public const int MaxStartDay = 28;

    public static MonthRange Of(int year, int month, int startDay = 1)
    {
        if (year is < 2000 or > 2100)
        {
            throw new ValidationFailedException("Year must be between 2000 and 2100.");
        }

        if (month is < 1 or > 12)
        {
            throw new ValidationFailedException("Month must be between 1 and 12.");
        }

        startDay = Math.Clamp(startDay, MinStartDay, MaxStartDay);

        var named = new DateOnly(year, month, 1);

        // Late start days belong to the month they end in, so begin one month earlier.
        var startMonth = startDay > 15 ? named.AddMonths(-1) : named;
        var start = new DateOnly(startMonth.Year, startMonth.Month, startDay);
        var end = start.AddMonths(1).AddDays(-1);

        return new MonthRange(year, month, start, end);
    }

    /// <summary>The budgeting month that contains <paramref name="date"/>.</summary>
    public static MonthRange Containing(DateOnly date, int startDay = 1)
    {
        var range = Of(date.Year, date.Month, startDay);

        if (date < range.Start)
        {
            var previous = new DateOnly(date.Year, date.Month, 1).AddMonths(-1);
            return Of(previous.Year, previous.Month, startDay);
        }

        if (date > range.End)
        {
            var next = new DateOnly(date.Year, date.Month, 1).AddMonths(1);
            return Of(next.Year, next.Month, startDay);
        }

        return range;
    }

    public MonthRange Previous()
    {
        var previous = new DateOnly(Year, Month, 1).AddMonths(-1);
        return Of(previous.Year, previous.Month, Start.Day);
    }

    public int DayCount => End.DayNumber - Start.DayNumber + 1;

    public bool Contains(DateOnly date) => date >= Start && date <= End;
}
