using System.Net;

namespace Exchange.Rates.API.Models;

public record ExchangeRateResponse(decimal ExchangeRate, decimal BidPrice, decimal AskPrice);

public record ExceptionResponse(HttpStatusCode StatusCode, string Description);