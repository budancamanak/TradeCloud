namespace Common.Core.Enums;

public enum SignalType
{
    OpenLong = 1,
    CloseLong = 2,
    OpenShort = 3,
    CloseShort = 4
}

public static class SignalTypeExtensions
{
    public static SignalType ToSignalType(this int value)
    {
        return value switch
        {
            1 => SignalType.OpenLong,
            2 => SignalType.CloseLong,
            3 => SignalType.OpenShort,
            4 => SignalType.CloseShort,
            _ => throw new ArgumentOutOfRangeException(nameof(value), value, null)
        };
    }

    public static SignalType ToSignalType(this string value)
    {
        return value switch
        {
            "Open Long" => SignalType.OpenLong,
            "Close Long" => SignalType.CloseLong,
            "Open Short" => SignalType.OpenShort,
            "Close Short" => SignalType.CloseShort,
            _ => throw new ArgumentOutOfRangeException(nameof(value), value, null)
        };
    }

    public static string GetStringRepresentation(this SignalType signalType)
    {
        return signalType switch
        {
            SignalType.OpenLong => "Open Long",
            SignalType.CloseLong => "Close Long",
            SignalType.OpenShort => "Open Short",
            SignalType.CloseShort => "Close Short",
            _ => throw new ArgumentOutOfRangeException(nameof(signalType), signalType, null)
        };
    }
}