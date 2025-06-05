using Common.Plugin.Abstraction;
using Common.Plugin.Models;
using Newtonsoft.Json;

namespace Worker.Plugins.FollowLineIndicator;

public class FollowLineIndicatorPluginParams : IParameters
{
    public int BBPeriod { get; set; }
    public int BBDeviation { get; set; }
    public int UseATRFilter { get; set; }
    public int ATRPeriod { get; set; }
    private FollowLineIndicatorPluginParamSet? _paramSet = null;

    public FollowLineIndicatorPluginParams()
    {
        
    }
    
    public IPluginParamSet GetParamSet()
    {
        _paramSet ??= new FollowLineIndicatorPluginParamSet();
        return _paramSet!;
    }

    public string GetStringRepresentation()
    {
        return $"{BBPeriod}, {BBDeviation}, {UseATRFilter}, {ATRPeriod}";
    }

    public string ToJson()
    {
        return JsonConvert.SerializeObject(this);
    }
}

public class FollowLineIndicatorPluginParamSet : IPluginParamSet
{
    public Param BBPeriod { get; set; }
    public Param BBDeviation { get; set; }
    public Param UseATRFilter { get; set; }
    public Param ATRPeriod { get; set; }

    public FollowLineIndicatorPluginParamSet()
    {
        BBPeriod = Param.Int.Range("BBPeriod", 1, 100, 1, 21);
        BBDeviation = Param.Int.Range("BBDeviation", 1, 5, 1, 1);
        UseATRFilter = Param.Int.List("UseATRFilter", 1, 0, 1);
        ATRPeriod = Param.Int.Range("ATRPeriod", 1, 25, 1, 5);
    }

    public string GetStringRepresentation()
    {
        return $"{BBPeriod}, {BBDeviation}, {UseATRFilter}, {ATRPeriod}";
    }

    public string ToJson()
    {
        return JsonConvert.SerializeObject(this);
    }
}