namespace Monefy.Api.Helpers;

/// <summary>Inclusive first/last day of a calendar month.</summary>
public readonly record struct MonthRange(int Year, int Month, DateOnly Start, DateOnly End)
{
    public static MonthRange Of(int year, int month)
    {
        if (year is < 2000 or > 2100)
        {
            throw new ValidationFailedException("Year must be between 2000 and 2100.");
        }

        if (month is < 1 or > 12)
        {
            throw new ValidationFailedException("Month must be between 1 and 12.");
        }

        var start = new DateOnly(year, month, 1);
        var end = start.AddMonths(1).AddDays(-1);

        return new MonthRange(year, month, start, end);
    }

    public int DayCount => End.Day;
}
