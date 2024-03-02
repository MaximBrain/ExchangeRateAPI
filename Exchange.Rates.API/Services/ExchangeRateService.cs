using Exchange.Rates.API.Models;

namespace Exchange.Rates.API.Services;

public record ExchangeRateService(IAlphaVantageApi AlphaVantageApi, IRepository RatesRepository)
{
    public async Task<CurrencyExchangeRate> GetRate(string from, string to)
    {
        var exchangeRates = await GetCurrentExchangeRate(from.ToUpperInvariant(), to.ToUpperInvariant());

        if (exchangeRates != null)
        {
            return exchangeRates;
        }
        
        return await Create(from, to);
    }

    public Task CreateRate(RateCreationRequest rateCreationRequest)
    {
        return AddToDatabase(rateCreationRequest);
    }

    public Task UpdateRate(CurrencyExchangeRate current, RateCreationRequest rateCreationRequest)
    {
        current.AskPrice = rateCreationRequest.AskPrice;
        current.BidPrice = rateCreationRequest.BidPrice;
        current.ExchangeRate = rateCreationRequest.ExchangeRate;
        current.EffectiveDate = DateTime.UtcNow;
        
        return RatesRepository.SaveChangesAsync();
    }

    public Task<CurrencyExchangeRate?> GetCurrentExchangeRate(string from, string to)
    {
        return RatesRepository.GetCurrentExchangeRate(from, to);
    }

    public Task DeleteRate(CurrencyExchangeRate current)
    {
        return RatesRepository.DeleteRate(current);
    }

    private async Task<CurrencyExchangeRate> Create(string from, string to)
    {
        var result = await AlphaVantageApi.GetRate(from, to);
        return await AddToDatabase(result.CurrencyExchangeRate!);
    }

    private CurrencyExchangeRate CreateCurrencyExchangeRate(
        Currency newFromCurrency,
        Currency newToCurrency,
        decimal askPrice,
        decimal bidPrice,
        decimal exchangeRate)
    {
        var fromCurrency = RatesRepository.AddCurrencyIfNotExists(newFromCurrency);
        var toCurrency = RatesRepository.AddCurrencyIfNotExists(newToCurrency);

        var currencyExchangeRate = new CurrencyExchangeRate
        {
            AskPrice = askPrice,
            BidPrice = bidPrice,
            EffectiveDate = DateTime.UtcNow,
            ExchangeRate = exchangeRate,
            FromCurrency = fromCurrency,
            ToCurrency = toCurrency
        };
        return currencyExchangeRate;
    }
    
    private async Task<CurrencyExchangeRate> AddToDatabase(AlphaVantageRate alphaVantageRate)
    {
        var newFromCurrency = CreateCurrency(alphaVantageRate.FromCurrencyCode, alphaVantageRate.FromCurrencyName);
        var newToCurrency = CreateCurrency(alphaVantageRate.ToCurrencyCode, alphaVantageRate.ToCurrencyName);
        
        var exchangeRate = CreateCurrencyExchangeRate(
            newFromCurrency, newToCurrency,
            decimal.Parse(alphaVantageRate.AskPrice),
            decimal.Parse(alphaVantageRate.BidPrice),
            decimal.Parse(alphaVantageRate.ExchangeRate));

        return await SaveExchangeRate(exchangeRate);
    }
    private async Task<CurrencyExchangeRate> AddToDatabase(RateCreationRequest rateCreationRequest)
    {
        var newFromCurrency = CreateCurrency(rateCreationRequest.FromCurrency.ToUpperInvariant());
        var newToCurrency = CreateCurrency(rateCreationRequest.ToCurrency.ToUpperInvariant());
        
        var exchangeRate = CreateCurrencyExchangeRate(
            newFromCurrency, newToCurrency,
            rateCreationRequest.AskPrice,
            rateCreationRequest.BidPrice,
            rateCreationRequest.ExchangeRate);

        return await SaveExchangeRate(exchangeRate);
    }
    private static Currency CreateCurrency(string currencyCode, string? currencyName = null)
    {
        return new Currency
        {
            CurrencyCode = currencyCode,
            CurrencyName = currencyName
        };
    }
    private Task<CurrencyExchangeRate> SaveExchangeRate(CurrencyExchangeRate exchangeRate)
    {
        return RatesRepository.AddToDatabase(exchangeRate);
    }
}