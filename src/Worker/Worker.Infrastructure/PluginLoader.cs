﻿using System.Reflection;
using Common.Application.Repositories;
using Common.Plugin.Abstraction;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Worker.Application.Abstraction;

namespace Worker.Infrastructure;

public class PluginLoader : IPluginLoader
{
    private readonly string _pluginFolder;
    private readonly IList<IPlugin> _plugins;
    private readonly ILogger<PluginHost> _logger;
    private readonly IPluginMessageBroker _messageBroker;
    private readonly IServiceScopeFactory scopeFactory;
    private readonly ICacheService cache;

    public PluginLoader(IServiceScopeFactory scopeFactory, IConfiguration configuration, ICacheService cache,
        IPluginMessageBroker messageBroker, ILogger<PluginHost> logger)
    {
        this.cache = cache;
        this.scopeFactory = scopeFactory;
        _logger = logger;
        _messageBroker = messageBroker;
        _pluginFolder = configuration["Plugins:Folder"] ??
                        $"{Path.GetDirectoryName(typeof(PluginLoader).Assembly.Location)}";
        _plugins = LoadPlugins();
    }

    public IList<IPlugin> Plugins() => _plugins;

    public IList<IPlugin> LoadPlugins()
    {
        var options = new EnumerationOptions
        {
            MatchCasing = MatchCasing.CaseInsensitive,
            IgnoreInaccessible = true,
            MatchType = MatchType.Simple,
            RecurseSubdirectories = true,
            ReturnSpecialDirectories = false
        };
        var pluginPaths = Directory.GetFiles(_pluginFolder, "*.dll", options);
        return pluginPaths.SelectMany(pluginPath =>
        {
            using var dynamicContext = new AssemblyResolver(pluginPath);
            return LoadPluginFromFile(pluginPath, dynamicContext.Assembly);
        }).ToList();
    }

    private IEnumerable<IPlugin> LoadPluginFromFile(string pluginPath, Assembly assembly)
    {
        using var scope = scopeFactory.CreateScope();
        var pluginLogger = scope.ServiceProvider.GetRequiredService<ILogger<IPlugin>>();
        var count = 0;
        foreach (var type in assembly.GetTypes())
        {
            if (!type.GetInterfaces().Any(inf => inf.Name.Equals(nameof(IPlugin)))) continue;
            if (type.IsAbstract) continue;
            // var result =
            //     Activator.CreateInstance(type, args: [pluginLogger, _messageBroker, cache]) as IPlugin;
            // if (result == null)
            // {
            //     continue;
            // }

            var result = CreatePlugin(type);
            if (result == null) continue;
            // result.UseLogger(_logger);
            result.UseMessageBroker(_messageBroker);
            count++;
            yield return result;
        }

        if (count == 0) yield break;
        _logger.LogInformation("Loading {} plugins from: {}", count, pluginPath);
    }

    private IPlugin? CreatePlugin(Type type)
    {
        using var scope = scopeFactory.CreateScope();
        var pluginLogger = scope.ServiceProvider.GetRequiredService<ILogger<IPlugin>>();
        var result =
            Activator.CreateInstance(type, args: [pluginLogger, _messageBroker, cache]) as IPlugin;
        if (result == null)
        {
            return null;
        }

        // result.UseLogger(_logger);
        result.UseMessageBroker(_messageBroker);
        return result;
    }

    public IPlugin CreatePlugin(string identifier)
    {
        var pluginTemplate = _plugins.FirstOrDefault(f => f.GetPluginInfo().Identifier == identifier);
        if (pluginTemplate == null) throw new Exception($"Failed to find plugin with {identifier}");
        // var plugin = pluginTemplate.Duplicate();
        // // plugin.UseLogger(_logger);
        // plugin.UseMessageBroker(_messageBroker);
        // return plugin;
        var plugin = CreatePlugin(pluginTemplate.GetPluginType());
        if (plugin == null) throw new Exception($"Failed to find plugin with {identifier}");
        return plugin;
    }
}