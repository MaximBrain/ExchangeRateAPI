using System.Globalization;
using Exchange.Rates.API.Extensions;

namespace Exchange.Rates.API.Services;

public record ExchangeRateService(AlphaVantageApi AlphaVantageApi, ApplicationDbContext ApplicationDbContext)
{
    public async Task<decimal> GetRate(string from, string to)
    {
        var exchangeRates = ApplicationDbContext.ExchangeRates.Where(d =>
            d.FromCurrency!.CurrencyCode == from && d.ToCurrency!.CurrencyCode == to).ToList();

        if (!exchangeRates.Any())
        {
            var result = await AlphaVantageApi.GetRate(from, to);
            await AddToDatabase(from, to, result);
            return decimal.Parse(result.CurrencyExchangeRate.ExchangeRate, CultureInfo.InvariantCulture);
        }

        if (exchangeRates.Count > 1)
        {
            throw new InvalidOperationException("Multiple exchange rates found.");
        }

        return exchangeRates.Single().ExchangeRate;
    }

    private async Task AddToDatabase(string from, string to, AlphaVantageResponse result)
    {
        var newFromCurrency = new Currency
        {
            CurrencyCode = result.CurrencyExchangeRate.FromCurrencyCode,
            CurrencyName = result.CurrencyExchangeRate.FromCurrencyName
        };
        
        var newToCurrency = new Currency
        {
            CurrencyCode = result.CurrencyExchangeRate.ToCurrencyCode,
            CurrencyName = result.CurrencyExchangeRate.ToCurrencyName
        };
        
        var fromCurrency = ApplicationDbContext.Set<Currency>().AddIfNotExists(newFromCurrency,
            x => x.CurrencyCode == result.CurrencyExchangeRate.FromCurrencyCode);
        var toCurrency = ApplicationDbContext.Set<Currency>().AddIfNotExists(newToCurrency,
            x => x.CurrencyCode == result.CurrencyExchangeRate.ToCurrencyCode);

        var exchangeRate = new CurrencyExchangeRate
        {
            AskPrice = decimal.Parse(result.CurrencyExchangeRate.AskPrice),
            BidPrice = decimal.Parse(result.CurrencyExchangeRate.BidPrice),
            EffectiveDate = DateTime.Parse(result.CurrencyExchangeRate.LastRefreshed),
            ExchangeRate = decimal.Parse(result.CurrencyExchangeRate.ExchangeRate),
            FromCurrency = fromCurrency,
            ToCurrency = toCurrency
        };

        ApplicationDbContext.ExchangeRates.Add(exchangeRate);

        await ApplicationDbContext.SaveChangesAsync();
    }

    public async Task CreateRate(string from, string to, decimal amount)
    {
        throw new NotImplementedException();
    }
}