using Common.Plugin.Abstraction;

namespace Worker.Plugins.MovingAverage;

public class GoldenDeathCrossCollector(IPlugin plugin, IPluginMessageBroker messageBroker)
{
    private readonly Dictionary<int, List<GoldenDeathCrossComputationData>> _data = new();
    
    public void Collect(int pluginId, int rowId, double slow, double fast,DateTime date)
    {
        if (!_data.ContainsKey(rowId))
            _data.Add(rowId, []);
        _data[rowId].Add(new GoldenDeathCrossComputationData { Fast = fast, Slow = slow, RowId = rowId });
        messageBroker.OnPluginComputation(plugin, pluginId, rowId, "slow", slow, date);
        messageBroker.OnPluginComputation(plugin, pluginId, rowId, "fast", fast, date);
        Analyze(rowId);
    }

    private void Analyze(int tillRow)
    {
        
    }
}

internal struct GoldenDeathCrossComputationData
{
    public int RowId { get; set; }
    public double Slow { get; set; }
    public double Fast { get; set; }
}