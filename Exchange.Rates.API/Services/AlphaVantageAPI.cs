using Exchange.Rates.API.Models;
using Microsoft.Extensions.Options;

namespace Exchange.Rates.API.Services;

public class AlphaVantageException(string errorMessage) : Exception(errorMessage);

public record AlphaVantageApi(HttpClient HttpClient, IOptions<AlphaVantageSettings> Settings, ILogger<AlphaVantageApi> Logger)
{
    private const string CurrencyExchangeRateFunction = "CURRENCY_EXCHANGE_RATE";

    public async Task<AlphaVantageResponse> GetRate(string from, string to)
    {
        var settings = Settings.Value;
        var response = await HttpClient.GetAsync(
            $"query?function={CurrencyExchangeRateFunction}&from_currency={from}&to_currency={to}&apikey={settings.ApiKey}");

        response.EnsureSuccessStatusCode();
        
        var result = await response.Content.ReadFromJsonAsync<AlphaVantageResponse>() ?? throw new InvalidOperationException("Failed to get rate.");
        if (!string.IsNullOrWhiteSpace(result.ErrorMessage))
        {
            throw new AlphaVantageException(result.ErrorMessage);
        }

        return result;
    }
}

