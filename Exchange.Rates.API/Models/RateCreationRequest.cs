namespace Exchange.Rates.API.Models;

public record RateCreationRequest
{
    public required string FromCurrency { get; init; }
    public required string ToCurrency { get; init; }
    public required decimal ExchangeRate { get; init; }
    public required decimal BidPrice { get; init; }
    public required decimal AskPrice { get; init; }
}