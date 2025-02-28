using AutoMapper;
using Common.Core.DTOs;
using FluentAssertions;
using Market.Application.Abstraction.Services;
using Market.Application.Mappers;
using Tests.Common.Data;
using Market.Infrastructure.Services;

namespace Infrastructure.Tests.ServicesTests;

[TestFixture]
public class PriceFetchCalculatorServiceTests
{
    private IPriceFetchCalculatorService _calculatorService;
    private IMapper _mapper;

    [SetUp]
    public void Setup()
    {
        _calculatorService = new PriceFetchCalculatorService();
        var mappingConfig = new MapperConfiguration(mc => { mc.AddProfile(new PriceMappingProfile()); });
        _mapper = mappingConfig.CreateMapper();
    }

    [Test]
    public void PriceFetchCalculator_FetchNeeded_EarlierStartDate_ShouldReturnTrue()
    {
        var prices = _mapper.Map<IList<PriceDto>>(MarketServiceTestData.Instance.Prices);
        var fetchNeed = _calculatorService.CheckPriceFetchIfNeeded(prices,
            MarketServiceTestData.Now.AddMinutes(-200),
            MarketServiceTestData.Now.AddMinutes(-1));
        fetchNeed.Should().BeTrue();
    }

    [Test]
    public void PriceFetchCalculator_FetchNeeded_LaterEndDate_ShouldReturnTrue()
    {
        var prices = _mapper.Map<IList<PriceDto>>(MarketServiceTestData.Instance.Prices);
        var fetchNeed = _calculatorService.CheckPriceFetchIfNeeded(prices,
            MarketServiceTestData.Now.AddMinutes(-2),
            MarketServiceTestData.Now.AddMinutes(5));
        fetchNeed.Should().BeTrue();
    }

    [Test]
    public void PriceFetchCalculator_FetchNeeded_ShouldReturnFalse()
    {
        var prices = _mapper.Map<IList<PriceDto>>(MarketServiceTestData.Instance.Prices);
        var fetchNeed = _calculatorService.CheckPriceFetchIfNeeded(prices,
            MarketServiceTestData.Now.AddHours(-2),
            MarketServiceTestData.Now.AddHours(-1));
        fetchNeed.Should().BeFalse();
    }

    [Test]
    public void PriceFetchCalculator_FetchNeeded_MissingStartDate_ShouldReturnTrue()
    {
        var prices = _mapper.Map<IList<PriceDto>>(MarketServiceTestData.Instance.Prices);
        var fetchNeed = _calculatorService.CheckPriceFetchIfNeeded(prices,
            default,
            MarketServiceTestData.Now.AddMinutes(-1));
        fetchNeed.Should().BeTrue();
    }

    [Test]
    public void PriceFetchCalculator_FetchNeeded_MissingEndDate_ShouldReturnTrue()
    {
        var prices = _mapper.Map<IList<PriceDto>>(MarketServiceTestData.Instance.Prices);
        var fetchNeed = _calculatorService.CheckPriceFetchIfNeeded(prices,
            MarketServiceTestData.Now.AddMinutes(-2),
            default);
        fetchNeed.Should().BeTrue();
    }

    [Test]
    public void PriceFetchCalculator_FetchNeeded_MissingPriceData_ShouldReturnTrue()
    {
        IList<PriceDto> data = null;
        var fetchNeed = _calculatorService.CheckPriceFetchIfNeeded(data,
            MarketServiceTestData.Now.AddMinutes(-2),
            MarketServiceTestData.Now.AddMinutes(-1));
        fetchNeed.Should().BeTrue();
    }
}