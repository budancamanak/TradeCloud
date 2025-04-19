using Microsoft.Extensions.Logging;
using Serilog.Events;

namespace Common.Logging;

public static class Extensions
{
    public static LogEventLevel ToLogLevel(this string level)
    {
        if (Enum.TryParse(new ReadOnlySpan<char>(level.ToCharArray()), true, out LogEventLevel lv))
            return lv;
        return LogEventLevel.Fatal;
        // LogEventLevel lv = Enum.Parse<LogEventLevel>(level, true);
        // return lv;
    }
}