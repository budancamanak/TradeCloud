using Common.Core.DTOs;
using Common.Core.Enums;
using Common.Core.Models;
using FluentAssertions;
using FluentValidation;
using Hangfire;
using Hangfire.Common;
using Hangfire.States;
using Market.Application.Abstraction.Services;
using Market.Application.Exceptions;
using Market.Application.Features.GetPricesForPlugin;
using Market.Application.Features.GetPricesForPlugin.Request;
using Market.Application.Features.GetPricesForPlugin.Validator;
using Market.Application.Models;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Tests.Common;
using Tests.Common.Data;

namespace Application.Tests.FeatureTests.GetPricesForPlugin;

[TestFixture]
public class GetPricesForPluginQueryHandlerTests : AbstractLoggableTest
{
    // todo implement tickerservice
    // todo write tickerservice tests
    private ILogger<GetPricesForPluginQueryHandler> _logger;
    private IBackgroundJobClient _jobClient;
    private GetPricesForPluginQueryValidator _validator;
    private IPriceService _priceService;
    private IPriceFetchCalculatorService _fetchCalculatorService;
    private IPriceFetchJob _fetchJob;
    private ITickerService _tickerService;

    [SetUp]
    public override void SetUp()
    {
        base.SetUp();
        _logger = _loggerFactory.CreateLogger<GetPricesForPluginQueryHandler>();
        _validator = new GetPricesForPluginQueryValidator();

        _priceService = Substitute.For<IPriceService>();
        _tickerService = Substitute.For<ITickerService>();

        _tickerService.CreateFetchRequest(Arg.Any<GetPricesForPluginQuery>())
            .Returns(new PriceFetchRequest(1, "CacheKey", 1, "binance", "CRV/USDT", Timeframe.Day1));
        _tickerService.CreateFetchRequest(Arg.Is<GetPricesForPluginQuery>(i => i.TickerId == 15))
            .Throws(new ArgumentNullException("Ticker", "TickerNotFound"));
        _tickerService.CreateFetchRequest(Arg.Is<GetPricesForPluginQuery>(i => i.TickerId <= 0))
            .Throws(new ArgumentException("TickerNotFound"));

        // - Arg.Is<int>(i => i < 10))
        _priceService.GetPricesForPluginAsync(Arg.Any<string>(), Arg.Any<int>(), Arg.Is<int>(i => i == 1),
            Arg.Any<Timeframe>(),
            Arg.Any<DateTime>(),
            Arg.Any<DateTime>()).Returns(new List<PriceDto>());
        _priceService.GetPricesForPluginAsync(Arg.Any<string>(), Arg.Any<int>(), Arg.Is<int>(i => i == 2),
            Arg.Any<Timeframe>(),
            Arg.Any<DateTime>(),
            Arg.Any<DateTime>()).Returns(new List<PriceDto>()
        {
            new(MarketServiceTestData.Now, 1, 1, 1, 1,1)
        });


        _fetchCalculatorService = Substitute.For<IPriceFetchCalculatorService>();
        _fetchCalculatorService.CheckPriceFetchIfNeeded(Arg.Is<IList<PriceDto>>(f => f.Count == 0), Arg.Any<DateTime>(),
            Arg.Any<DateTime>()).Returns(true);
        _fetchCalculatorService.CheckPriceFetchIfNeeded(Arg.Is<IList<PriceDto>>(f => f.Count > 0), Arg.Any<DateTime>(),
            Arg.Any<DateTime>()).Returns(false);


        _fetchJob = Substitute.For<IPriceFetchJob>();
        _fetchJob.StartFetchPrices(Arg.Any<IList<PriceFetchPages>>(), Arg.Any<PriceFetchRequest>(),
            Arg.Any<CancellationToken>()).Returns(Task.FromResult(MethodResponse.Success("")));

        _jobClient = Substitute.For<IBackgroundJobClient>();
        _jobClient.Create(Arg.Any<Job>(), Arg.Any<IState>()).Returns("identifier");
    }

    [Test]
    public async Task GetPricesForPluginQueryHandler_OK()
    {
        var x = new GetPricesForPluginQueryHandler(_priceService, _tickerService, _fetchCalculatorService, _validator,
            _fetchJob, _jobClient,
            _logger);

        var start = MarketServiceTestData.Now.AddHours(-12);
        var end = MarketServiceTestData.Now.AddHours(-6);
        var query = new GetPricesForPluginQuery(1, 1, Timeframe.Hour1.GetStringRepresentation(), start, end);


        var result = await x.Handle(query, CancellationToken.None);
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    [Test]
    public async Task GetPricesForPluginQueryHandler_WhenNoFetchNeeded_OK()
    {
        var x = new GetPricesForPluginQueryHandler(_priceService, _tickerService, _fetchCalculatorService, _validator,
            _fetchJob, _jobClient,
            _logger);

        var start = MarketServiceTestData.Now.AddHours(-12);
        var end = MarketServiceTestData.Now.AddHours(-6);
        var query = new GetPricesForPluginQuery(2, 2, Timeframe.Hour1.GetStringRepresentation(), start, end);


        var result = await x.Handle(query, CancellationToken.None);
        result.Should().NotBeNull();
        result.Should().NotBeEmpty();
    }

    [Test]
    [TestCase(0)]
    [TestCase(-1)]
    public async Task GetPricesForPluginQueryHandler_WhenGivenInvalidTickerId_ShouldFail(int tickerId)
    {
        var x = new GetPricesForPluginQueryHandler(_priceService, _tickerService, _fetchCalculatorService, _validator,
            _fetchJob, _jobClient,
            _logger);

        var start = MarketServiceTestData.Now.AddHours(-12);
        var end = MarketServiceTestData.Now.AddHours(-6);
        var query = new GetPricesForPluginQuery(tickerId, 1, Timeframe.Hour1.GetStringRepresentation(), start, end);

        var action = async () => { await x.Handle(query, CancellationToken.None); };
        await action.Should().ThrowAsync<ValidationException>();
    }

    [Test]
    public async Task GetPricesForPluginQueryHandler_WhenGivenWrongTimeframe_ShouldFail()
    {
        var x = new GetPricesForPluginQueryHandler(_priceService, _tickerService, _fetchCalculatorService, _validator,
            _fetchJob, _jobClient,
            _logger);

        var start = MarketServiceTestData.Now.AddHours(-12);
        var end = MarketServiceTestData.Now.AddHours(-6);
        var query = new GetPricesForPluginQuery(1, 1, default, start, end);

        var action = async () => { await x.Handle(query, CancellationToken.None); };
        await action.Should().ThrowAsync<ArgumentOutOfRangeException>();
    }

    [Test]
    public async Task GetPricesForPluginQueryHandler_WhenGivenWrongTicker_ShouldFail()
    {
        // var x = new GetPricesForPluginQueryHandler(priceService, tickerService, fetchCalculatorService, validator,
        //     fetchJob, jobClient,
        //     logger);
        //
        // var start = MarketServiceTestData.Now.AddHours(-12);
        // var end = MarketServiceTestData.Now.AddHours(-6);
        // var query = new GetPricesForPluginQuery(1, Timeframe.Hour1.GetStringRepresentation(), start, end);
        //
        // var action = async () => { await x.Handle(query, CancellationToken.None); };
        // await action.Should().ThrowAsync<ValidationException>();
        var x = new GetPricesForPluginQueryHandler(_priceService, _tickerService, _fetchCalculatorService, _validator,
            _fetchJob, _jobClient,
            _logger);

        var start = MarketServiceTestData.Now.AddHours(-12);
        var end = MarketServiceTestData.Now.AddHours(-6);
        var query = new GetPricesForPluginQuery(15, 1, Timeframe.Hour1.GetStringRepresentation(), start, end);

        var action = async () => { await x.Handle(query, CancellationToken.None); };
        await action.Should().ThrowAsync<ArgumentNullException>();
    }

    [Test]
    public async Task GetPricesForPluginQueryHandler_WhenGivenNullStartDate_ShouldFail()
    {
        var x = new GetPricesForPluginQueryHandler(_priceService, _tickerService, _fetchCalculatorService, _validator,
            _fetchJob, _jobClient,
            _logger);

        var start = default(DateTime);
        var end = MarketServiceTestData.Now.AddHours(-6);
        var query = new GetPricesForPluginQuery(1, 1, Timeframe.Hour1.GetStringRepresentation(), start, end);

        var action = async () => { await x.Handle(query, CancellationToken.None); };
        await action.Should().ThrowAsync<ArgumentNullException>();
    }

    [Test]
    public async Task GetPricesForPluginQueryHandler_WhenGivenNullEndDate_ShouldFail()
    {
        var x = new GetPricesForPluginQueryHandler(_priceService, _tickerService, _fetchCalculatorService, _validator,
            _fetchJob, _jobClient,
            _logger);

        var start = MarketServiceTestData.Now.AddHours(-6);
        var end = default(DateTime);
        var query = new GetPricesForPluginQuery(1, 1, Timeframe.Hour1.GetStringRepresentation(), start, end);

        var action = async () => { await x.Handle(query, CancellationToken.None); };
        await action.Should().ThrowAsync<ArgumentNullException>();
    }

    [Test]
    public async Task GetPricesForPluginQueryHandler_WhenGivenWrongDates_ShouldFail()
    {
        var x = new GetPricesForPluginQueryHandler(_priceService, _tickerService, _fetchCalculatorService, _validator,
            _fetchJob, _jobClient,
            _logger);

        var start = MarketServiceTestData.Now.AddHours(-6);
        var end = MarketServiceTestData.Now.AddHours(-16); // reversed times
        var query = new GetPricesForPluginQuery(1, 1, Timeframe.Hour1.GetStringRepresentation(), start, end);

        var action = async () => { await x.Handle(query, CancellationToken.None); };
        await action.Should().ThrowAsync<NoPageCalculatedException>();
    }
}