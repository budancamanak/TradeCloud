using Common.Plugin.Abstraction;

namespace Common.Plugin.Models;

public class ParamSet : IPluginParamSet
{
    public List<Param> Parameters { get; private set; }

    public ParamSet()
    {
        Parameters = new List<Param>();
    }

    public ParamSet Add(Param param)
    {
        Parameters.Add(param);
        return this;
    }

    public string GetStringRepresentation()
    {
        return "";
    }

    public string ToJson()
    {
        return "";
    }
}