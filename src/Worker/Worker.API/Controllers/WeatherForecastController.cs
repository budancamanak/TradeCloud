using Common.Application.Repositories;
using Common.Core.Models;
using Common.Plugin.Abstraction;
using Microsoft.AspNetCore.Mvc;
using Worker.Application.Abstraction;


namespace Worker.API.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController(ILogger<WeatherForecastController> logger, IPluginHost host, ICacheService cache)
    : ControllerBase
{
    [HttpGet("/PluginList")]
    public async Task<IEnumerable<IPlugin.PluginInfo>> GetPlugins()
    {
        var plugins = await cache.GetAsync<List<IPlugin.PluginInfo>>("AvailablePlugins");
        // var plugins = host.Plugins();
        logger.LogWarning(plugins[0].Identifier);
        logger.LogWarning(plugins[0].Name);
        return plugins;
    }

    [HttpPost("/RunPlugin")]
    public async Task<MethodResponse> RunPlugin([FromBody] string identifier)
    {
        var mr = await host.CanNewPluginRun();
        if (!mr.IsSuccess) return mr;
        return MethodResponse.Success("implement this");
        /***
         * todo this api will be grpc controller
         * todo validate input
         * todo check if plugin queue is full
         *     todo if queue is full, return error-
         * todo make grpc request to market (1)
         * todo if prices returned
         *     todo get plugin from cache
         *     todo start hangfire job to run plugin
         *     todo send plugin started event-rabbitmq
         * todo if empty returned
         *     todo put plugin to waiting cache
         *     todo send plugin waiting data event-rabbitmq
         *
         * todo listen for rabbitmq- price fetched event
         *     todo when price fetch ended
         *         todo remove plugin from waiting cache
         *         todo go to 1 (above)
         *
         * todo listen for rabbitmq- price fetch failed event
         *     todo send plugin failed event-rabbitmq-reason fetch failure
         *     todo remove plugin from waiting cache
         *
         * todo when a plugin finishes - success/failure
         *     todo get next plugin from backend -> grpc call, retrieve from plugin executions
         *
         *
         * todo need plugin data from backend
         *      todo pluginId, pluginIdentifier, startDate, endDate, tickerId, timeFrame
         *      todo executionParas, tradeParams->for real/demo money
         *
         * todo tradeParams: later
         * todo executionParams: each plugin will have its own param set
         * todo                  each plugin will return default param set
         */
    }
}