using Microsoft.Extensions.Logging;

namespace Common.Logging.Events.Worker;

public class PluginEvents
{
    public static readonly EventId BasePlugin = new EventId(1, "BasePlugin");
    public static readonly EventId BuiltInPlugin = new EventId(2, "BuiltInPlugin");
}