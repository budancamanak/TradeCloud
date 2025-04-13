namespace Common.Core.Models;

public record PluginInfo(string Name, string Identifier, string Version = "1.0.0")
{
}