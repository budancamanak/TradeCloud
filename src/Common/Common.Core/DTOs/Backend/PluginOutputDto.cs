using Common.Core.Enums;

namespace Common.Core.DTOs.Backend;

public class PluginOutputDto
{
    public string PluginName { get; set; }
    public string SignalType { get; set; }
    public DateTime SignalDate { get; set; }
}