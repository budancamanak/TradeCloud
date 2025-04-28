namespace Common.Core.Models;

public record PluginInfo(string Name, string Identifier, string Version = "1.0.0")
{
    public static PluginInfo NULL_PLUGIN = new PluginInfo("Not found", "");
}