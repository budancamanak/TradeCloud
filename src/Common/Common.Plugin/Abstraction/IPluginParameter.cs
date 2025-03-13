namespace Common.Plugin.Abstraction;

public interface IPluginParameter
{
    bool IsListParameter();
    Type ParameterType();
}