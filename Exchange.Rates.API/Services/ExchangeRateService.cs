using Exchange.Rates.API.Extensions;
using Exchange.Rates.API.Models;
using Microsoft.EntityFrameworkCore;

namespace Exchange.Rates.API.Services;

public record ExchangeRateService(AlphaVantageApi AlphaVantageApi, ApplicationDbContext ApplicationDbContext)
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

    public async Task<CurrencyExchangeRate> CreateRate(RateCreationRequest rateCreationRequest)
    {
        return await AddToDatabase(rateCreationRequest);
    }

    public async Task<CurrencyExchangeRate?> GetCurrentExchangeRate(string from, string to)
    {
        var exchangeRates = await ApplicationDbContext.ExchangeRates
            .FirstOrDefaultAsync(d => d.FromCurrency!.CurrencyCode == from
                                      && d.ToCurrency!.CurrencyCode == to);
        return exchangeRates;
    }

    private async Task<CurrencyExchangeRate> Create(string from, string to)
    {
        var result = await AlphaVantageApi.GetRate(from, to);
        return await AddToDatabase(result.CurrencyExchangeRate);
    }

    private async Task<CurrencyExchangeRate> AddToDatabase(AlphaVantageRate alphaVantageRate)
    {
        var newFromCurrency = new Currency
        {
            CurrencyCode = alphaVantageRate.FromCurrencyCode,
            CurrencyName = alphaVantageRate.FromCurrencyName
        };
        
        var newToCurrency = new Currency
        {
            CurrencyCode = alphaVantageRate.ToCurrencyCode,
            CurrencyName = alphaVantageRate.ToCurrencyName
        };
        
        var exchangeRate = CreateCurrencyExchangeRate(
            newFromCurrency, newToCurrency,
            decimal.Parse(alphaVantageRate.AskPrice),
            decimal.Parse(alphaVantageRate.BidPrice),
            decimal.Parse(alphaVantageRate.ExchangeRate),
            DateTime.Parse(alphaVantageRate.LastRefreshed));

        ApplicationDbContext.ExchangeRates.Add(exchangeRate);

        await ApplicationDbContext.SaveChangesAsync();

        return exchangeRate;
    }

    private async Task<CurrencyExchangeRate> AddToDatabase(RateCreationRequest rateCreationRequest)
    {
        var newFromCurrency = new Currency
        {
            CurrencyCode = rateCreationRequest.FromCurrency,
        };
        
        var newToCurrency = new Currency
        {
            CurrencyCode = rateCreationRequest.ToCurrency,
        };
        
        var exchangeRate = CreateCurrencyExchangeRate(
            newFromCurrency, newToCurrency,
            rateCreationRequest.AskPrice, rateCreationRequest.BidPrice,
            rateCreationRequest.ExchangeRate, DateTime.UtcNow);

        ApplicationDbContext.ExchangeRates.Add(exchangeRate);

        await ApplicationDbContext.SaveChangesAsync();

        return exchangeRate;
    }

    private CurrencyExchangeRate CreateCurrencyExchangeRate(
        Currency newFromCurrency,
        Currency newToCurrency,
        decimal askPrice,
        decimal bidPrice,
        decimal exchangeRate,
        DateTime effectiveDate)
    {
        var fromCurrency = ApplicationDbContext.Set<Currency>().AddIfNotExists(newFromCurrency,
            x => x.CurrencyCode == newFromCurrency.CurrencyCode);
        var toCurrency = ApplicationDbContext.Set<Currency>().AddIfNotExists(newToCurrency,
            x => x.CurrencyCode == newToCurrency.CurrencyCode);

        var currencyExchangeRate = new CurrencyExchangeRate
        {
            AskPrice = askPrice,
            BidPrice = bidPrice,
            EffectiveDate = effectiveDate,
            ExchangeRate = exchangeRate,
            FromCurrency = fromCurrency,
            ToCurrency = toCurrency
        };
        return currencyExchangeRate;
    }
}