﻿using Common.Core.Models;
using Common.Plugin.Abstraction;
using Worker.Application.Features.RunAnalysisRequested;
using Worker.Application.Features.RunPluginRequested;

namespace Worker.Application.Abstraction;

public interface IPluginHost
{
    IList<IPlugin> Plugins();
    bool AddPluginToQueue(RunPluginRequest request);
    bool AddAnalysisToQueue(RunAnalysisRequest requested);
    void RemovePluginFromQueue(int pluginId);
    RunPluginRequest GetRequestFor(int pluginId);
    Task<MethodResponse> RunPlugin(int pluginId);
    Tuple<IPlugin,string,string> GetPluginToRun(int requestExecutionId);
    Task<MethodResponse> CanNewPluginRun();
    MethodResponse IsPluginInQueue(int pluginId);
}