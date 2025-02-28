using Backend.Domain.Entities;
using Common.Application.Repositories;
using Common.Core.Enums;

namespace Backend.Application.Abstraction.Repositories;

public interface IPluginOutputRepository : IAsyncRepository<PluginOutput>
{
    Task<List<PluginOutput>> GetPluginOutputs(int pluginId);
    Task<List<PluginOutput>> GetPluginOutputsOfSignalType(int pluginId, SignalType signalType);
}