using AutoMapper;
using Common.Application.Repositories;
using Common.Application.Services;
using Common.Core.DTOs;
using Common.Core.Enums;
using FluentAssertions;
using Market.Application.Abstraction.Services;
using Tests.Common.Data;
using Market.Application.Mappers;
using Market.Application.Validators;
using Market.Domain.Entities;
using Market.Infrastructure.Repositories;
using Market.Infrastructure.Services;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using Testcontainers.Redis;
using Tests.Common;

namespace Infrastructure.Tests.ServicesTests;

[TestFixture]
public class PriceServiceTests : AbstractLoggableTest
{
    private IPriceService _priceService;
    private RedisContainer _redisContainer;
    private ICacheService _cacheService;
    private ConnectionMultiplexer _connection;
    private MarketDatabaseFixture _fixture;
    private IMapper _mapper;

    [OneTimeSetUp]
    public async Task OneTimeSetup()
    {
        _redisContainer = new RedisBuilder().Build();
        await _redisContainer.StartAsync();
        _connection = await ConnectionMultiplexer.ConnectAsync(_redisContainer.GetConnectionString());
        _cacheService = new RedisCacheService(_connection);
        var mappingConfig = new MapperConfiguration(mc => { mc.AddProfile(new PriceMappingProfile()); });
        _mapper = mappingConfig.CreateMapper();
    }

    [OneTimeTearDown]
    public async Task OneTimeTearDown()
    {
        await _connection.CloseAsync();
        await _redisContainer.DisposeAsync();
    }

    [SetUp]
    public override void SetUp()
    {
        base.SetUp();
        var validator = new PriceValidator();
        _fixture = new MarketDatabaseFixture("MarketDbPriceServiceTests");
        _fixture.SeedData();
        var repository = new PriceRepository(_fixture.DbContext, validator);
        var mappingConfig = new MapperConfiguration(mc => { mc.AddProfile(new PriceMappingProfile()); });
        var mapper = mappingConfig.CreateMapper();
        var logger = _loggerFactory.CreateLogger<PriceService>();
        _priceService = new PriceService(_cacheService, repository, mapper, logger);
    }

    [TearDown]
    public void Dispose()
    {
        _fixture.Dispose();
    }

    [Test]
    [Order(0)]
    public async Task PriceServiceTest_GetPricesForPlugin_ShouldReturnList()
    {
        var r = await _priceService.GetPricesForPluginAsync("TESTCACHEKEY", 2, 2, Timeframe.Hour1,
            MarketServiceTestData.Now.AddHours(-2),
            MarketServiceTestData.Now.AddHours(-1));
        r.ToList().Should().NotBeNull();
        r.ToList().Should().NotBeEmpty();
    }

    [Test]
    public async Task PriceServiceTest_WhenWrongTickerIdGiven_ShouldFail()
    {
        var r = await _priceService.GetPricesForPluginAsync("Pl2", 2, 1123, Timeframe.Hour1,
            MarketServiceTestData.Now.AddHours(-2),
            MarketServiceTestData.Now.AddHours(-1));
        r.ToList().Should().BeEmpty();
    }

    [Test]
    public async Task PriceServiceTest_WhenNegativeTickerIdGiven_ShouldFail()
    {
        var action = async () =>
        {
            await _priceService.GetPricesForPluginAsync("Pl3", 1, -15, Timeframe.Hour1,
                MarketServiceTestData.Now.AddHours(-2),
                MarketServiceTestData.Now.AddHours(-1));
        };
        await action.Should().ThrowAsync<ArgumentException>();
    }

    [Test]
    public async Task PriceServiceTest_WhenWrongStartDateGiven_ShouldFail()
    {
        var action = async () =>
        {
            await _priceService.GetPricesForPluginAsync("Pl3", 1, 1, Timeframe.Hour1,
                default,
                MarketServiceTestData.Now.AddHours(-1));
        };
        await action.Should().ThrowAsync<ArgumentNullException>();
    }

    [Test]
    public async Task PriceServiceTest_SaveMissingPrices()
    {
        var prices = new List<Price>()
        {
            new Price
            {
                TickerId = 2, Close = 1, Open = 1, High = 1, Low = 1, Timeframe = Timeframe.Hour1,
                Timestamp = MarketServiceTestData.Now.AddHours(-25)
            },
            new Price
            {
                TickerId = 2, Close = 12, Open = 14, High = 12, Low = 13, Timeframe = Timeframe.Hour1,
                Timestamp = MarketServiceTestData.Now.AddHours(-26)
            },
        };
        var mr = await _priceService.SaveMissingPricesAsync(prices, "");
        mr.IsSuccess.Should().BeTrue();
        mr.Id.Should().BeGreaterThan(0);
    }

    [Test]
    public async Task PriceServiceTest_SaveMissingPrices_ThenReturnCached()
    {
        var pluginId = "PriceServiceTest_SaveMissingPrices_ThenReturnCached";
        var prices = new List<Price>()
        {
            new Price
            {
                TickerId = 2, Close = 1, Open = 1, High = 1, Low = 1, Timeframe = Timeframe.Hour1,
                Timestamp = MarketServiceTestData.Now.AddHours(-25)
            },
            new Price
            {
                TickerId = 2, Close = 12, Open = 14, High = 12, Low = 13, Timeframe = Timeframe.Hour1,
                Timestamp = MarketServiceTestData.Now.AddHours(-26)
            },
        };
        var mr = await _priceService.SaveMissingPricesAsync(prices, pluginId);
        mr.IsSuccess.Should().BeTrue();
        mr.Id.Should().BeGreaterThan(0);
        var items = await _cacheService.GetAsync<List<PriceDto>>(pluginId);
        items.Should().NotBeNullOrEmpty();
        for (int i = 0; i < prices.Count; i++)
        {
            prices[i].Timestamp.Should().Be(items[i].Timestamp);
            prices[i].Open.Should().Be(items[i].Open);
            prices[i].Close.Should().Be(items[i].Close);
            prices[i].High.Should().Be(items[i].High);
            prices[i].Low.Should().Be(items[i].Low);
        }
    }

    [Test]
    public async Task PriceServiceTest_ShouldReturnCachedList()
    {
        var pluginId = "PriceServiceTest_ShouldReturnCachedList";
        var current = await _cacheService.GetAsync<List<PriceDto>>(pluginId);
        current.Should().BeNullOrEmpty();
        var items = _mapper.Map<List<PriceDto>>(_fixture.DbContext.Prices.ToList());
        await _cacheService.SetAsync(pluginId, items, TimeSpan.FromDays(1));
        var r = await _priceService.GetPricesForPluginAsync(pluginId, 1, 2, Timeframe.Hour1,
            MarketServiceTestData.Now.AddHours(-2),
            MarketServiceTestData.Now.AddHours(-1));
        r.Should().NotBeNull();
        r.Count.Should().Be(items.Count);
        for (int i = 0; i < r.Count; i++)
        {
            r[i].Timestamp.Should().Be(items[i].Timestamp);
            r[i].Open.Should().Be(items[i].Open);
            r[i].Close.Should().Be(items[i].Close);
            r[i].High.Should().Be(items[i].High);
            r[i].Low.Should().Be(items[i].Low);
        }
    }

    [Test]
    public async Task PriceServiceTest_SaveMissingPrices_WhenNullPriceInfoGiven_ShouldFail()
    {
        var action = async () => { await _priceService.SaveMissingPricesAsync(null, ""); };
        await action.Should().ThrowAsync<ArgumentNullException>();
    }

    [Test]
    public async Task PriceServiceTest_SaveMissingPrices_WhenEmptyPriceInfoGiven_ShouldFail()
    {
        var action = async () => { await _priceService.SaveMissingPricesAsync(new List<Price>(), ""); };
        await action.Should().ThrowAsync<ArgumentOutOfRangeException>();
    }

    [Test]
    public async Task PriceServiceTest_SaveMissingPrices_WhenInvalidPriceInfoGiven_ShouldFail()
    {
        var prices = new List<Price>()
        {
            new Price
            {
                TickerId = -1, Close = 1, Open = 1, High = 1, Low = 1, Timeframe = Timeframe.Day1,
                Timestamp = MarketServiceTestData.Now
            },
            new Price
            {
                TickerId = 1, Timestamp = MarketServiceTestData.Now
            }
        };
        var action = async () => { await _priceService.SaveMissingPricesAsync(prices, ""); };
        await action.Should().ThrowAsync<ArgumentException>();
    }
}