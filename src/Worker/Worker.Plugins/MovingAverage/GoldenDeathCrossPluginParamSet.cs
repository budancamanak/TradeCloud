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

public class GoldenDeathCrossPluginParamSet : IPluginParamSet
{
    public GoldenDeathCrossPluginParamSet()
    {
    }

    public GoldenDeathCrossPluginParamSet(int fast, int slow)
    {
        this.FastMovingAverage = NumericParameter<int>.IntParameter("FastMovingAverage", fast);
        this.SlowMovingAverage = NumericParameter<int>.IntParameter("SlowMovingAverage", slow);
    }

    // public int FastMovingAvg { get; set; }
    // public int SlowMovingAvg { get; set; }
    public NumericParameter<int> FastMovingAverage { get; set; }
    public NumericParameter<int> SlowMovingAverage { get; set; }

    public string GetStringRepresentation()
    {
        return $"{FastMovingAverage}, {SlowMovingAverage}";
    }

    public string ToJson()
    {
        return JsonConvert.SerializeObject(this);
    }
}