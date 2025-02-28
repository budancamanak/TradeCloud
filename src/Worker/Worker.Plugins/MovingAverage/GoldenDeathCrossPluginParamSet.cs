using Common.Plugin.Abstraction;

namespace Worker.Plugins.MovingAverage;

public class GoldenDeathCrossPluginParamSet : IPluginParamSet
{
    public int FastMovingAvg { get; set; }
    public int SlowMovingAvg { get; set; }

    public GoldenDeathCrossPluginParamSet()
    {
    }

    public GoldenDeathCrossPluginParamSet(int fast, int slow)
    {
        this.FastMovingAvg = fast;
        this.SlowMovingAvg = slow;
    }

    public string GetStringRepresentation()
    {
        return $"FastMovingAvg:{FastMovingAvg}, SlowMovingAvg:{SlowMovingAvg}";
    }
}