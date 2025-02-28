using ccxt;
using Common.Core.DTOs;
using Common.Core.Enums;
using Common.Core.Models;
using FluentAssertions;
using Market.Application.Exceptions;
using Market.Application.Models;
using Market.Application.Services;
using Market.Application.Features.SaveFetchedPrices.Request;
using Market.Application.Utilities;
using Market.Infrastructure.Services;
using MediatR;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Tests.Common;
using Tests.Common.Data;

namespace Infrastructure.Tests.ServicesTests;

[TestFixture]
public class PriceFetchJobTests : AbstractLoggableTest
{
    private PriceFetchJob _fetchJob;
    private ILogger<PriceFetchJob> _logger;

    [SetUp]
    public override void SetUp()
    {
        base.SetUp();
        _logger = _loggerFactory.CreateLogger<PriceFetchJob>();

        var mediatorMock = Substitute.For<IMediator>();
        mediatorMock.Send(Arg.Any<PriceFetchCompletedCommand>(), CancellationToken.None)
            .Returns(MethodResponse.Success(""));
        _fetchJob = new PriceFetchJob(mediatorMock, _logger);
    }

    [Test]
    [Ignore("Skipped for faster test execution")]
    public async Task PriceFetch_Fetch_OK()
    {
        // Arrange
        var start = MarketServiceTestData.Now.AddHours(-12);
        var end = MarketServiceTestData.Now.AddHours(-6);
        var pages = PriceFetchPageCalculator.ToPages(Timeframe.Hour1, start, end, Constants.PriceFetchLimit);
        // var query = new GetPricesForPluginQuery(1, Timeframe.Hour1.GetStringRepresentation(), start, end);
        var query = new PriceFetchRequest(1, "CACHE_KEY", 1, "binance", "CRV/USDT", Timeframe.Hour1);

        // Act
        var result = await _fetchJob.StartFetchPrices(pages, query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
    }

    [Test]
    public async Task PriceFetch_WrongExchangeName_ShouldFail()
    {
        // Arrange
        var start = MarketServiceTestData.Now.AddHours(-12);
        var end = MarketServiceTestData.Now.AddHours(-6);
        var pages = PriceFetchPageCalculator.ToPages(Timeframe.Hour1, start, end, Constants.PriceFetchLimit);
        // var query = new GetPricesForPluginQuery(1, "CRV/USDT", "UnknownExchange", Timeframe.Hour1,
        //     start, end);
        var query = new PriceFetchRequest(1, "CACHE_KEY",1, "UnknownExchange", "CRV/USDT", Timeframe.Hour1);

        // Act
        Func<Task> getAction = async () => { await _fetchJob.StartFetchPrices(pages, query, CancellationToken.None); };

        // Assert
        await getAction.Should().ThrowAsync<SystemException>();
    }

    [Test]
    public async Task PriceFetch_WrongSymbolName_ShouldFail()
    {
        // Arrange
        var start = MarketServiceTestData.Now.AddHours(-12);
        var end = MarketServiceTestData.Now.AddHours(-6);
        var pages = PriceFetchPageCalculator.ToPages(Timeframe.Hour1, start, end, Constants.PriceFetchLimit);
        // var query = new GetPricesForPluginQuery(1, "CRVXXUSDT", "binance", Timeframe.Hour1, start, end);
        var query = new PriceFetchRequest(1, "CACHE_KEY",1, "binance", "CRVXXUSDT", Timeframe.Hour1);


        // Act
        Func<Task> getAction = async () => { await _fetchJob.StartFetchPrices(pages, query, CancellationToken.None); };

        // Assert
        await getAction.Should().ThrowAsync<ExchangeError>();
    }

    [Test]
    public async Task PriceFetch_WrongPageSinceParameter_ShouldFail()
    {
        // Arrange
        var start = MarketServiceTestData.Now.AddHours(-12);
        var end = MarketServiceTestData.Now.AddHours(-6);
        // var query = new GetPricesForPluginQuery(1, "CRV/USDT", "binance", Timeframe.Hour1, start, end);
        var query = new PriceFetchRequest(1, "CACHE_KEY",1, "binance", "CRV/USDT", Timeframe.Hour1);
        var pages = new List<PriceFetchPages> { new(-1, Constants.PriceFetchLimit) };

        // Act
        Func<Task> getAction = async () => { await _fetchJob.StartFetchPrices(pages, query, CancellationToken.None); };

        // Assert
        await getAction.Should().ThrowAsync<ExchangeError>();
    }

    [Test]
    public async Task PriceFetch_OnCancellationToken_ShouldFail()
    {
        // Arrange
        var start = MarketServiceTestData.Now.AddHours(-12);
        var end = MarketServiceTestData.Now.AddHours(-6);
        var pages = PriceFetchPageCalculator.ToPages(Timeframe.Hour1, start, end, Constants.PriceFetchLimit);
        var query = new PriceFetchRequest(1, "CACHE_KEY",1, "binance", "CRV/USDT", Timeframe.Hour1);
        var cancelToken = new CancellationToken(true);

        // Act
        Func<Task> getAction = async () => { await _fetchJob.StartFetchPrices(pages, query, cancelToken); };

        // Assert
        await getAction.Should().ThrowAsync<OperationCanceledException>();
    }

    [Test]
    public async Task PriceFetch_WhenNoFetchedPrices_OnFinish_ShouldFail()
    {
        _fetchJob.ActiveRequest = new PriceFetchRequest(1, "CACHE_KEY",1, "binance", "CRV/USDT", Timeframe.Hour1);
        _fetchJob.FetchedPrices = new List<PriceDto>();

        // Act
        Func<Task> fetchAction = async () => { await _fetchJob.OnFinish(); };

        // Assert
        await fetchAction.Should().ThrowAsync<NoPriceFetchedException>();
    }

    [Test]
    public async Task PriceFetch_WhenNullQuery_OnFinish_ShouldFail()
    {
        _fetchJob.FetchedPrices = new List<PriceDto>()
        {
            new(MarketServiceTestData.Now, 1, 2, 1, 1,1)
        };

        // Act
        Func<Task> fetchAction = async () => { await _fetchJob.OnFinish(); };

        // Assert
        await fetchAction.Should().ThrowAsync<ArgumentNullException>();
    }

    [Test]
    public async Task PriceFetch_WhenNoException_OnError_ShouldFail()
    {
        _fetchJob.ActiveRequest = new PriceFetchRequest(1, "CACHE_KEY",1, "binance", "CRV/USDT", Timeframe.Hour1);


        // Act
        Func<Task> fetchAction = async () => { await _fetchJob.OnError(null); };

        // Assert
        await fetchAction.Should().ThrowAsync<ArgumentNullException>();
    }

    [Test]
    public async Task PriceFetch_WhenNoActiveRequest_OnError_ShouldFail()
    {
        _fetchJob.ActiveRequest = null;
        var exc = new Exception("Test Exception");


        // Act
        Func<Task> fetchAction = async () => { await _fetchJob.OnError(exc); };

        // Assert
        await fetchAction.Should().ThrowAsync<ArgumentNullException>();
    }

    [Test]
    public async Task PriceFetch_WhenExceptionHappened_OnError_ShouldThrow()
    {
        _fetchJob.ActiveRequest = new PriceFetchRequest(1, "CACHE_KEY",1, "binance", "CRV/USDT", Timeframe.Hour1);
        var exc = new TestException("Test Exception");


        // Act
        Func<Task> fetchAction = async () => { await _fetchJob.OnError(exc); };

        // Assert
        await fetchAction.Should().ThrowExactlyAsync<TestException>();
    }

    class TestException(string msg) : Exception(msg);
}