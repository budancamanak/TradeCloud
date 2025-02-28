using Common.Plugin.Abstraction;

namespace Backend.Application.Abstraction.Services;

public interface IPluginExecutionHelper
{
    Task<List<string>> GenerateParamSetArray(IPlugin plugin);
}