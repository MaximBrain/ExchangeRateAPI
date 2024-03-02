namespace Exchange.Rates.API.Models;

public record RateCreationRequest
{
    public string FromCurrency { get; init; }
    public string ToCurrency { get; init; }
    public decimal ExchangeRate { get; init; }
    public decimal BidPrice { get; init; }
    public decimal AskPrice { get; init; }
}