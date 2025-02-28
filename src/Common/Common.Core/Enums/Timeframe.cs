namespace Common.Core.Enums;

// todo change timeframe to string from long
// todo switch case from timeframe to long

/// <summary>
/// timeframe enum. values are in seconds not milliseconds
/// </summary>
public enum Timeframe
{
    Minute5 = 300,
    Minute15 = 900,
    Minute30 = 1800,
    Hour1 = 3600,
    Hour2 = 7200,
    Hour4 = 14_400,
    Hour8 = 28_800,
    Day1 = 86_400,
    Day3 = 259_200,
    Week = 604_800,
    Month = 2_592_000
}

public static class TimeframeExtensions
{
    public static long GetMilliseconds(this Timeframe timeframe)
    {
        return (long)timeframe * 1000;
    }

    public static Timeframe TimeFrameFromString(this string timeframe)
    {
        return timeframe switch
        {
            "5m" => Timeframe.Minute5,
            "15m" => Timeframe.Minute15,
            "30m" => Timeframe.Minute30,
            "1h" => Timeframe.Hour1,
            "2h" => Timeframe.Hour2,
            "4h" => Timeframe.Hour4,
            "8h" => Timeframe.Hour8,
            "1d" => Timeframe.Day1,
            "3d" => Timeframe.Day3,
            "1w" => Timeframe.Week,
            "1M" => Timeframe.Month,
            _ => throw new ArgumentOutOfRangeException(nameof(timeframe), timeframe, null)
        };
    }

    public static string GetStringRepresentation(this Timeframe timeframe)
    {
        return timeframe switch
        {
            Timeframe.Minute5 => "5m",
            Timeframe.Minute15 => "15m",
            Timeframe.Minute30 => "30m",
            Timeframe.Hour1 => "1h",
            Timeframe.Hour2 => "2h",
            Timeframe.Hour4 => "4h",
            Timeframe.Hour8 => "8h",
            Timeframe.Day1 => "1d",
            Timeframe.Day3 => "3d",
            Timeframe.Week => "1w",
            Timeframe.Month => "1M",
            _ => throw new ArgumentOutOfRangeException(nameof(timeframe), timeframe, null)
        };
    }
}