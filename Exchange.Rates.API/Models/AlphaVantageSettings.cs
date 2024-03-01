namespace Exchange.Rates.API.Models;

public record AlphaVantageSettings()
{
    public string? BaseUrl { get; init; }
    public string? ApiKey { get; init; }
}