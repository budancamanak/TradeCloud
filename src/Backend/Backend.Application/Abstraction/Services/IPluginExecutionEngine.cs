﻿using Backend.Domain.Entities;
using Common.Plugin.Abstraction;
using Common.Plugin.Models;

namespace Backend.Application.Abstraction.Services;

public interface IPluginExecutionEngine
{
    // Task<IPlugin> GetNextWaitingPlugin();
    // Task<MethodResponse> UpdatePluginStatus(int id, PluginStatus status);
    // Task<MethodResponse> UpdatePluginProgress(int id, double progress);
    // /// <summary>
    // /// Tries to run the plugin. If there's no slot to run, adds the plugin to waiting queue
    // /// </summary>
    // /// <param name="plugin"></param>
    // /// <returns></returns>
    // Task<MethodResponse> RunPlugin(IPlugin plugin);

    List<PluginExecution> GeneratePluginExecutions(AnalysisExecution execution);
    List<Param> GenerateParameters(AnalysisExecution execution);
}