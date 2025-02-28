using Common.Core.Enums;

namespace Backend.Domain.Entities;

public class PluginOutput
{
    public int Id { get; set; }
    public int PluginId { get; set; }
    public SignalType PluginSignal { get; set; }
    public DateTime SignalDate { get; set; }
    public DateTime CreatedDate { get; set; } = DateTime.Now;

    public PluginExecution PluginExecution { get; set; }

    public override string ToString()
    {
        return $"[{Id}] [{PluginId}] {PluginSignal} {SignalDate}";
    }
}