namespace Common.Plugin.Abstraction;

public interface IPluginParamSet
{
    string GetStringRepresentation();
    string ToJson();
}