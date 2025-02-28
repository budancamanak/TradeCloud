using Common.Application.Repositories;
using Common.Application.Services;
using FluentAssertions;
using Market.Infrastructure.Services;
using StackExchange.Redis;
using Testcontainers.Redis;

namespace Infrastructure.Tests.ServicesTests;

[TestFixture]
public class RedisCacheServiceTests
{
    private RedisContainer _redisContainer;
    private ICacheService _cacheService;
    private ConnectionMultiplexer _connection;

    [OneTimeSetUp]
    public async Task Setup()
    {
        _redisContainer = new RedisBuilder().Build();
        await _redisContainer.StartAsync();

        _connection = await ConnectionMultiplexer.ConnectAsync(_redisContainer.GetConnectionString());
        _cacheService = new RedisCacheService(_connection);
    }

    [OneTimeTearDown]
    public async Task Dispose()
    {
        await _connection.CloseAsync();
        await _redisContainer.DisposeAsync();
    }

    [Test]
    public async Task SetAsync_ShouldStoreDataInCache()
    {
        // Arrange
        var key = "test-key";
        var value = "test-value";
        var expiration = TimeSpan.FromMinutes(1);

        // Act
        await _cacheService.SetAsync(key, value, expiration);
        var cachedValue = await _cacheService.GetAsync<string>(key);

        // Assert
        value.Should().Be(cachedValue);
    }

    [Test]
    public async Task GetAsync_ShouldReturnNull_WhenKeyDoesNotExist()
    {
        // Arrange
        var key = "nonexistent-key";

        // Act
        var cachedValue = await _cacheService.GetAsync<string>(key);

        // Assert
        cachedValue.Should().BeNull();
    }

    [Test]
    public async Task KeyShouldExpireAfterGivenTime()
    {
        // Arrange
        var key = "expiring-key";
        var value = "expiring-value";
        var expiration = TimeSpan.FromSeconds(1);

        // Act
        await _cacheService.SetAsync(key, value, expiration);

        // Wait for expiration
        await Task.Delay(1500);
        var cachedValue = await _cacheService.GetAsync<string>(key);

        // Assert
        cachedValue.Should().BeNull();
    }

    [Test]
    public async Task RemoveAsync_ShouldDeleteKeyFromCache()
    {
        // Arrange
        var key = "removable-key";
        var value = "removable-value";

        await _cacheService.SetAsync(key, value, TimeSpan.FromMinutes(1));

        // Act
        await _cacheService.RemoveAsync(key);
        var cachedValue = await _cacheService.GetAsync<string>(key);

        // Assert
        cachedValue.Should().BeNull();
    }
}