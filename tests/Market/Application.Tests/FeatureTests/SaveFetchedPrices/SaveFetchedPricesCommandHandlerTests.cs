using AutoMapper;
using Common.Core.DTOs;
using Common.Core.Enums;
using Common.Core.Models;
using Common.Messaging.Abstraction;
using Common.Messaging.Events;
using FluentAssertions;
using FluentValidation;
using Market.Application.Abstraction.Services;
using Market.Application.Features.SaveFetchedPrices;
using Market.Application.Features.SaveFetchedPrices.Request;
using Market.Application.Features.SaveFetchedPrices.Validator;
using Market.Application.Mappers;
using Market.Domain.Entities;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Tests.Common;
using Tests.Common.Data;

namespace Application.Tests.FeatureTests.SaveFetchedPrices;

[TestFixture]
public class SaveFetchedPricesCommandHandlerTests : AbstractLoggableTest
{
    private ILogger<SaveFetchedPricesCommandHandler> _logger;
    private IValidator<PriceFetchCompletedCommand> _validator;
    private IPriceService _priceService;
    private IMapper _mapper;
    private IEventBus _bus;


    [SetUp]
    public override void SetUp()
    {
        base.SetUp();
        var mappingConfig = new MapperConfiguration(mc => { mc.AddProfile(new PriceMappingProfile()); });
        _mapper = mappingConfig.CreateMapper();
        _logger = _loggerFactory.CreateLogger<SaveFetchedPricesCommandHandler>();
        _validator = new PriceFetchCompletedValidator();

        _bus = Substitute.For<IEventBus>();
        _bus.When(x => x.PublishAsync(Arg.Any<IntegrationEvent>())).Do(_ => { });

        _priceService = Substitute.For<IPriceService>();

        _priceService
            .SaveMissingPricesAsync(
                Arg.Any<List<Price>>(),
                Arg.Is<string>("SuccessfulPluginId"))
            .Returns(MethodResponse.Success("done"));
        _priceService
            .SaveMissingPricesAsync(Arg.Any<List<Price>>(), Arg.Is<string>("NullPricesPluginId"))
            .ThrowsAsync<ArgumentNullException>();
        _priceService
            .SaveMissingPricesAsync(Arg.Any<List<Price>>(), Arg.Is<string>("EmptyPricesPluginId"))
            .ThrowsAsync<ArgumentOutOfRangeException>();
        _priceService
            .SaveMissingPricesAsync(Arg.Any<List<Price>>(), Arg.Is<string>("AllInvalidPricesPluginId"))
            .ThrowsAsync<ArgumentException>();
    }

    [Test]
    public async Task GetPricesForPluginQueryHandler_OK()
    {
        var command = new PriceFetchCompletedCommand(new List<PriceDto>
        {
            new(MarketServiceTestData.Now, 1, 1, 1, 1,1)
        }, 1, "SuccessfulPluginId", 1, Timeframe.Hour1);

        var handler = new SaveFetchedPricesCommandHandler(_priceService, _mapper, _bus, _validator, _logger);


        var result = await handler.Handle(command, CancellationToken.None);
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
    }

    [Test]
    public async Task GetPricesForPluginQueryHandler_WhenFailsToSaveNullPriceList_ShouldFail()
    {
        var command = new PriceFetchCompletedCommand(new List<PriceDto>
        {
            new(MarketServiceTestData.Now, 1, 1, 1, 1,1)
        }, 1, "NullPricesPluginId", 1, Timeframe.Hour1);

        var handler = new SaveFetchedPricesCommandHandler(_priceService, _mapper, _bus, _validator, _logger);
        var action = async () => { await handler.Handle(command, CancellationToken.None); };
        await action.Should().ThrowAsync<ArgumentNullException>();
    }

    [Test]
    public async Task GetPricesForPluginQueryHandler_WhenFailsToSaveEmptyPriceList_ShouldFail()
    {
        var command = new PriceFetchCompletedCommand(new List<PriceDto>
        {
            new(MarketServiceTestData.Now, 1, 1, 1, 1,1)
        }, 1, "EmptyPricesPluginId", 1, Timeframe.Hour1);

        var handler = new SaveFetchedPricesCommandHandler(_priceService, _mapper, _bus, _validator, _logger);
        var action = async () => { await handler.Handle(command, CancellationToken.None); };
        await action.Should().ThrowAsync<ArgumentOutOfRangeException>();
    }

    [Test]
    public async Task GetPricesForPluginQueryHandler_WhenFailsToSaveInvalidPriceList_ShouldFail()
    {
        var command = new PriceFetchCompletedCommand(new List<PriceDto>
        {
            new()
        }, 1, "AllInvalidPricesPluginId", 1, Timeframe.Hour1);

        var handler = new SaveFetchedPricesCommandHandler(_priceService, _mapper, _bus, _validator, _logger);
        var action = async () => { await handler.Handle(command, CancellationToken.None); };
        await action.Should().ThrowAsync<ArgumentException>();
    }
}