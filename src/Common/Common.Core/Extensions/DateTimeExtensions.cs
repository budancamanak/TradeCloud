using Ardalis.GuardClauses;

namespace Common.Core.Extensions;

public static class DateTimeExtensions
{
    private const string DateKeyFormat = "dd-MM-yyyy HH:mm";

    public static long TotalMilliseconds(this DateTime date)
    {
        return (long)(date - new DateTime(1970, 1, 1)).TotalMilliseconds;
    }

    public static DateTime FitDateToTimeFrame(this DateTime date, long timeFrameMilli, bool lowerBound)
    {
        Guard.Against.NullDate(date);
        var millis = date.TotalMilliseconds();
        if (lowerBound)
            millis = (long)Math.Floor((double)millis / timeFrameMilli) * timeFrameMilli;
        else
            millis = (long)Math.Ceiling((double)millis / timeFrameMilli) * timeFrameMilli;
        return DateTime.UnixEpoch.AddMilliseconds(millis);
    }

    public static string ToReadableString(this DateTime date)
    {
        return date.ToString(DateKeyFormat);
    }
}