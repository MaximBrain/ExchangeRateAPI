using System.Text.Json.Serialization;

public record AlphaVantageRate
{
    [JsonPropertyName("1. From_Currency Code")]
    public string FromCurrencyCode { get; init; }

    [JsonPropertyName("2. From_Currency Name")]
    public string FromCurrencyName { get; init; }

    [JsonPropertyName("3. To_Currency Code")]
    public string ToCurrencyCode { get; init; }

    [JsonPropertyName("4. To_Currency Name")]
    public string ToCurrencyName { get; init; }

    [JsonPropertyName("5. Exchange Rate")]
    public string ExchangeRate { get; init; }

    [JsonPropertyName("6. Last Refreshed")]
    public string LastRefreshed { get; init; }

    [JsonPropertyName("7. Time Zone")] public string TimeZone { get; init; }

    [JsonPropertyName("8. Bid Price")] public string BidPrice { get; init; }

    [JsonPropertyName("9. Ask Price")] public string AskPrice { get; init; }
}

public record AlphaVantageResponse
{
    [JsonPropertyName("Realtime Currency Exchange Rate")]
    public AlphaVantageRate? CurrencyExchangeRate { get; init; }
    
    [JsonPropertyName("Error Message")]
    public string? ErrorMessage { get; init; }
}