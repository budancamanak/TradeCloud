using Common.Plugin.Abstraction;
using Common.Plugin.Models;
using Newtonsoft.Json;

namespace Worker.Plugins.WaveTrend;

public class WaveTrendPluginParams : IParameters
{
    public int ChannelLength { get; set; }
    public int AverageLength { get; set; }
    public int OverSoldLevel { get; set; }
    public int OverBoughtLevel { get; set; }
    public double CiMultiple { get; set; }
    public int WaveTrend2Length { get; set; }
    public int ApSrc { get; set; }

    private WaveTrendPluginParamSet? _paramSet = null;


    public IPluginParamSet GetParamSet()
    {
        _paramSet ??= new WaveTrendPluginParamSet();
        return _paramSet!;
    }

    public string GetStringRepresentation()
    {
        return
            $"{ChannelLength}, {AverageLength}, {OverSoldLevel}, {OverBoughtLevel}, {CiMultiple}, {WaveTrend2Length}, {ApSrc}";
    }

    public string ToJson()
    {
        return JsonConvert.SerializeObject(this);
    }
}

public class WaveTrendPluginParamSet : IPluginParamSet
{
    public Param ChannelLength { get; set; }
    public Param AverageLength { get; set; }
    public Param OverSoldLevel { get; set; }
    public Param OverBoughtLevel { get; set; }
    public Param CiMultiple { get; set; }
    public Param WaveTrend2Length { get; set; }
    public Param ApSrc { get; set; }

    public WaveTrendPluginParamSet()
    {
        ChannelLength = Param.Int.Range("ChannelLength", 5, 50, 1, 10);
        AverageLength = Param.Int.Range("AverageLength", 10, 200, 1, 21);
        OverSoldLevel = Param.Int.Range("OverSoldLevel", -100, 0, 1, -50);
        // 0:close, 1: high, 2: low, 3: open, 4: hl3
        ApSrc = Param.Int.List("ApSrc", 0, 0, 1, 2, 3, 4);
        OverBoughtLevel = Param.Int.Range("OverBoughtLevel", 0, 100, 1, 50);
        CiMultiple = Param.Int.Range("CiMultiple", 1, 25, 1, 5);
        WaveTrend2Length = Param.Int.Range("WaveTrend2Length", 1, 25, 1, 4);
    }

    public string GetStringRepresentation()
    {
        return
            $"{ChannelLength}, {AverageLength}, {OverSoldLevel}, {OverBoughtLevel}, {CiMultiple}, {WaveTrend2Length}, {ApSrc}";
    }

    public string ToJson()
    {
        return JsonConvert.SerializeObject(this);
    }
}