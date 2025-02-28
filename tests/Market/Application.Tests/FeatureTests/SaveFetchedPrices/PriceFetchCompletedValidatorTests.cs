using AutoMapper;
using Common.Core.DTOs;
using Common.Core.Enums;
using FluentAssertions;
using Market.Application.Features.SaveFetchedPrices.Request;
using Market.Application.Features.SaveFetchedPrices.Validator;
using Market.Application.Mappers;
using Tests.Common.Data;

namespace Application.Tests.FeatureTests.SaveFetchedPrices;

[TestFixture]
public class PriceFetchCompletedValidatorTests
{
    private PriceFetchCompletedValidator _validator;
    private IMapper _mapper;

    [SetUp]
    public void SetUp()
    {
        var mappingConfig = new MapperConfiguration(mc => { mc.AddProfile(new PriceMappingProfile()); });
        _mapper = mappingConfig.CreateMapper();
        _validator = new PriceFetchCompletedValidator();
    }

    [Test]
    public async Task PriceFetchCompletedValidator_OK()
    {
        var testPrices = _mapper.Map<List<PriceDto>>(MarketServiceTestData.Instance.Prices);
        var query = new PriceFetchCompletedCommand(testPrices, 1,"pluginId", 1, Timeframe.Hour1);
        var result = await _validator.ValidateAsync(query);
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Test]
    [TestCase("", -1)]
    [TestCase("pl", -1)]
    [TestCase("pl", 0)]
    public async Task PriceFetchCompletedValidator_WhenGivenMissingValues_ShouldFail(string pluginId, int tickerId)
    {
        var testPrices = _mapper.Map<List<PriceDto>>(MarketServiceTestData.Instance.Prices);
        var query = new PriceFetchCompletedCommand(testPrices,1, pluginId, tickerId, Timeframe.Hour1);
        var result = await _validator.ValidateAsync(query);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().NotBeEmpty();
    }
}