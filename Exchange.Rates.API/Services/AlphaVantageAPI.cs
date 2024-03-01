using Exchange.Rates.API.Models;
using Microsoft.Extensions.Options;

namespace Exchange.Rates.API.Services;

public record AlphaVantageApi(HttpClient HttpClient, IOptions<AlphaVantageSettings> Settings)
{
    public async Task<AlphaVantageResponse> GetRate(string from, string to)
    {
        var settings = Settings.Value;
        var response = await HttpClient.GetAsync(
            $"query?function=CURRENCY_EXCHANGE_RATE&from_currency={from}&to_currency={to}&apikey={settings.ApiKey}");

        response.EnsureSuccessStatusCode();
        
        return await response.Content.ReadFromJsonAsync<AlphaVantageResponse>() ?? throw new InvalidOperationException("Failed to get rate.");
    }
}