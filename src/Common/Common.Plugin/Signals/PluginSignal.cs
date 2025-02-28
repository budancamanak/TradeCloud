using Common.Core.Enums;

namespace Common.Plugin.Signals;

public sealed class PluginSignal
{
    public int TickerId { get; set; }
    public DateTime SignalDate { get; set; }
    public SignalType SignalType { get; set; }

    public PluginSignal()
    {
    }

    private PluginSignal(int tickerId, SignalType type, DateTime date)
    {
        TickerId = tickerId;
        SignalType = type;
        SignalDate = date;
    }

    public static PluginSignal OpenLong(int tickerId, DateTime? signalDate = null)
    {
        signalDate ??= DateTime.UtcNow;
        return new PluginSignal(tickerId, SignalType.OpenLong, signalDate.Value);
    }

    public static PluginSignal OpenShort(int tickerId, DateTime? signalDate = null)
    {
        signalDate ??= DateTime.UtcNow;
        return new PluginSignal(tickerId, SignalType.OpenShort, signalDate.Value);
    }

    public static PluginSignal CloseLong(int tickerId, DateTime? signalDate = null)
    {
        signalDate ??= DateTime.UtcNow;
        return new PluginSignal(tickerId, SignalType.CloseLong, signalDate.Value);
    }

    public static PluginSignal CloseShort(int tickerId, DateTime? signalDate = null)
    {
        signalDate ??= DateTime.UtcNow;
        return new PluginSignal(tickerId, SignalType.CloseShort, signalDate.Value);
    }

    public override string ToString()
    {
        return $"Plugin signalled of {SignalType} for Ticker[{TickerId}] @ {SignalDate}";
    }
}