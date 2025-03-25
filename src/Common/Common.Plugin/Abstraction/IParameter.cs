using Common.Plugin.Models;

namespace Common.Plugin.Abstraction;

public interface IParameters
{
    IPluginParamSet GetParamSet();
    string GetStringRepresentation();
    string ToJson();
}