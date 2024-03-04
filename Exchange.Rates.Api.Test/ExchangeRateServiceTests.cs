using Exchange.Rates.API;
using Exchange.Rates.API.Models;
using Exchange.Rates.API.Services;
using Moq;

namespace Exchange.Rates.Api.Test;

public class ExchangeRateServiceTests
{
    // Arrange
    private const string FromCurrency = "USD", ToCurrency = "EUR";

    private Mock<IAlphaVantageApi> _alphaVantageApiMock = null!;
    private Mock<IRepository> _ratesRepoMock = null!;
    private ExchangeRateService _service = null!;

    [SetUp]
    public void Setup()
    {
        _alphaVantageApiMock = new Mock<IAlphaVantageApi>();
        _ratesRepoMock = new Mock<IRepository>();
        _service = new ExchangeRateService(_alphaVantageApiMock.Object, _ratesRepoMock.Object);
    }

    [Test]
    public async Task GetRate_ShouldReturnCurrentExchangeRate_WhenExists()
    {
        var currentRate = new CurrencyExchangeRate
        {
            // Assumes the object declaration
            // Initial values
        };

        _ratesRepoMock.Setup(r => r.GetCurrentExchangeRate(FromCurrency, ToCurrency)).ReturnsAsync(currentRate);

        // Act
        var result = await _service.GetRate(FromCurrency, ToCurrency);

        // Assert
        Assert.That(result, Is.EqualTo(currentRate));
    }

    [Test]
    public async Task CreateRate_ShouldAddToDatabase()
    {
        // Arrange
        var rateCreationRequest = new RateCreationRequest
        {
            // Assumes the object declaration 
            FromCurrency = FromCurrency,
            ToCurrency = ToCurrency
        };

        _ratesRepoMock.Setup(r => r.AddCurrencyIfNotExists(It.IsAny<Currency>())).Returns(new Currency());

        // Act
        await _service.CreateRate(rateCreationRequest);

        // Assert
        _ratesRepoMock.Verify(r => r.AddToDatabase(It.IsAny<CurrencyExchangeRate>()), Times.Once);
    }

    [Test]
    public async Task UpdateRate_ShouldUpdateCurrentRate()
    {
        // Arrange
        var currentRate = new CurrencyExchangeRate
        {
            // Assumes the object declaration 
            BidPrice = 1,
            AskPrice = 1,
            ExchangeRate = 1,
        };

        var rateCreationRequest = new RateCreationRequest
        {
            // Assumes the object declaration 
            FromCurrency = FromCurrency,
            ToCurrency = ToCurrency,
            BidPrice = 2,
            AskPrice = 2,
            ExchangeRate = 2
        };

        // Act
        await _service.UpdateRate(currentRate, rateCreationRequest);

        // Assert
        _ratesRepoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        Assert.Multiple(() =>
        {
            Assert.That(currentRate.AskPrice, Is.EqualTo(rateCreationRequest.AskPrice));
            Assert.That(currentRate.BidPrice, Is.EqualTo(rateCreationRequest.BidPrice));
            Assert.That(currentRate.ExchangeRate, Is.EqualTo(rateCreationRequest.ExchangeRate));
        });
    }

    [Test]
    public async Task DeleteRate_ShouldDeleteCurrentRate()
    {
        // Arrange
        var currentRate = new CurrencyExchangeRate
        {
            // Assumes the object declaration 
            // Initial values
        };

        // Act
        await _service.DeleteRate(currentRate);

        // Assert
        _ratesRepoMock.Verify(r => r.DeleteRate(currentRate), Times.Once);
    }
}