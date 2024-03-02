namespace Exchange.Rates.API.Models;

public record RateCreationRequest
{
    public required string FromCurrency
    {
        get => FromCurrency;
        init => value = value.ToUpperInvariant();
    }

    public required string ToCurrency
    {
        get => ToCurrency;
        init => value = value.ToUpperInvariant();
    }
    
    public required decimal ExchangeRate { get; init; }
    public required decimal BidPrice { get; init; }
    public required decimal AskPrice { get; init; }
}