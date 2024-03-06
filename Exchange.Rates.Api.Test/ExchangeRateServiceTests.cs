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
    public async Task GetRate_WhenRateExists_ReturnsRateFromRepository()
    {
        // Arrange
        var from = "USD";
        var to = "EUR";
        var expectedRate = new CurrencyExchangeRate();

        _ratesRepoMock.Setup(x => x.GetCurrentExchangeRate(from, to))
            .ReturnsAsync(expectedRate);

        // Act
        var result = await _service.GetRate(from, to);

        // Assert
        Assert.That(result, Is.EqualTo(expectedRate));
        _alphaVantageApiMock.Verify(x => x.GetRate(from, to), Times.Never);
        _ratesRepoMock.Verify(x => x.GetCurrentExchangeRate(from, to), Times.Once);
    }

    [Test]
    public async Task GetRate_WhenRateDoesNotExist_ReturnsRateFromAlphaVantageApi()
    {
        // Arrange
        var from = "USD";
        var to = "EUR";
        var alphaVantageResponse = new AlphaVantageResponse
        {
            CurrencyExchangeRate = new AlphaVantageRate()
            {
                AskPrice = "1.1",
                BidPrice = "1.2",
                ExchangeRate = "1.3"
            }
        };

        var expectedRate = new CurrencyExchangeRate()
        {
            AskPrice = 1.1m,
            BidPrice = 1.2m,
            ExchangeRate = 1.3m
        };

        _ratesRepoMock.Setup(x =>
                x.GetCurrentExchangeRate(from, to))
            .ReturnsAsync((CurrencyExchangeRate?)null);
        _alphaVantageApiMock.Setup(x => x.GetRate(from, to)).ReturnsAsync(alphaVantageResponse);
        _ratesRepoMock.Setup(x =>
                x.AddToDatabase(It.IsAny<CurrencyExchangeRate>())).ReturnsAsync((CurrencyExchangeRate rate) => rate);

        // Act
        var actual = await _service.GetRate(from, to);

        Assert.Multiple(() =>
        {
            // Assert
            Assert.That(actual.BidPrice, Is.EqualTo(expectedRate.BidPrice));
            Assert.That(actual.AskPrice, Is.EqualTo(expectedRate.AskPrice));
            Assert.That(actual.ExchangeRate, Is.EqualTo(expectedRate.ExchangeRate));
        });

        _alphaVantageApiMock.Verify(x => x.GetRate(from, to), Times.Once);
        _ratesRepoMock.Verify(x => x.GetCurrentExchangeRate(from, to), Times.Once);
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