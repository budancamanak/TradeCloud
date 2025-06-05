using System.Configuration;
using Common.Application.Repositories;
using Common.Application.Services;
using Common.Core.DTOs;
using Common.Plugin.Math;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using Tests.Common;
using Worker.Plugins.FollowLineIndicator;

namespace Infrastructure.Tests;

[TestFixture]
public class PluginTests() : AbstractLoggableTest
{
    private ILogger<PluginTests> _logger;
    private ICacheService _cacheService;
    private ConnectionMultiplexer _connection;

    [OneTimeSetUp]
    public async Task Setup()
    {
        base.SetUp();
        _logger = _loggerFactory.CreateLogger<PluginTests>();
        var redisConnStr = "192.168.1.10:8379";
        _connection = await ConnectionMultiplexer.ConnectAsync(redisConnStr);
        _cacheService = new RedisCacheService(_connection);
    }

    [OneTimeTearDown]
    public async Task Dispose()
    {
        await _connection.CloseAsync();
    }


    [Test]
    public async Task TestFunc()
    {
        var priceinfo =
            await _cacheService.GetAsync<List<PriceDto>>("Prices:T2.TF3600000.S1746306000000.E1748638800000");
        // var output = new FollowLineIndicatorPlugin().r(new TradeMath(_cacheService, 999, priceinfo), 21, 1, 5, 1,
        //     priceinfo, _logger);
        priceinfo.Should().NotBeNull();
        // output.Should().NotBeNullOrEmpty();
        // output.Count.Should().BeGreaterThan(0);
    }
}