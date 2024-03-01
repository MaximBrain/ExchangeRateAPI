using System.Globalization;
using Exchange.Rates.API.Services;

namespace Exchange.Rates.API;

public record ExchangeRateService(AlphaVantageApi AlphaVantageApi)
{
    public async Task<decimal> GetRate(string from, string to)
    {
        var result = await AlphaVantageApi.GetRate(from, to);
        return decimal.Parse(result.CurrencyExchangeRate.ExchangeRate, CultureInfo.InvariantCulture);
    }

    public async Task CreateRate(string from, string to, decimal amount)
    {
        throw new NotImplementedException();
    }
}