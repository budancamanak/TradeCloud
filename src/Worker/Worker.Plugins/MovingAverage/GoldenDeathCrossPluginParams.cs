using Common.Plugin.Abstraction;
using Common.Plugin.Models;
using Newtonsoft.Json;

namespace Worker.Plugins.MovingAverage;

/*
 * Plugins will have param set -> will come from ui
 * Execution engine will separate param set and create param list
 *
 * IPluginParamSet -> Will have single & range parameter values -> to be used to generate parameters
 * IPluginParams -> Will contain primitive values to be used within plugins
 * Execution Engine -> Will use IPluginParamSet. do cartesian product for each parameter within. create json for each send them to plugin runs.
 *
 */

public class GoldenDeathCrossPluginParams : IParameters
{
    public int FastMovingAverage { get; set; }
    public int SlowMovingAverage { get; set; }
    private GoldenDeathCrossPluginParamSet? _paramSet = null;

    public GoldenDeathCrossPluginParams()
    {
    }

    public GoldenDeathCrossPluginParams(int fast, int slow)
    {
        this.FastMovingAverage = fast;
        this.SlowMovingAverage = slow;
    }


    public IPluginParamSet GetParamSet()
    {
        _paramSet ??= new GoldenDeathCrossPluginParamSet();
        return _paramSet!;
    }

    public string GetStringRepresentation()
    {
        return $"{FastMovingAverage}, {SlowMovingAverage}";
    }

    public string ToJson()
    {
        return JsonConvert.SerializeObject(this);
    }
}

public class GoldenDeathCrossPluginParamSet : IPluginParamSet
{
    public Param FastMovingAverage { get; set; }
    public Param SlowMovingAverage { get; set; }

    public GoldenDeathCrossPluginParamSet()
    {
        this.FastMovingAverage = Param.Int.Range("FastMovingAverage", 20, 50, 1, 20);
        this.SlowMovingAverage = Param.Int.Range("SlowMovingAverage", 50, 200, 1, 50);
    }

    public string GetStringRepresentation()
    {
        return $"{FastMovingAverage}, {SlowMovingAverage}";
    }

    public string ToJson()
    {
        return JsonConvert.SerializeObject(this);
    }
}