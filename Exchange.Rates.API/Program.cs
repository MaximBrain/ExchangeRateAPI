using Exchange.Rates.API.Extensions;
using Exchange.Rates.API.Middlewares;
using Exchange.Rates.API.Models;
using Exchange.Rates.API.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddApplicationServices(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Add middleware to handle exceptions
app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseHttpsRedirection();

// Get Rates with ExchangeRateService help and return result
app.MapGet("/rates/{from}/{to}",
        async (ExchangeRateService exchangeRateService, string from, string to)  =>
    {
        var result = await exchangeRateService.GetRate(from, to);
        
        return Results.Ok(new ExchangeRateResponse(result.ExchangeRate, result.BidPrice, result.AskPrice));
    })
    .WithName("GetRates")
    .WithOpenApi();

// Create Rates with ExchangeRateService help and return OK
app.MapPost("/rates",
        async (ExchangeRateService exchangeRateService, RateCreationRequest rateCreationRequest) =>
    {
        var current = await exchangeRateService.GetCurrentExchangeRate(rateCreationRequest.FromCurrency, rateCreationRequest.ToCurrency);
        if (current != null)
        {
            return Results.Conflict("Rate already exists.");
        }
        
        await exchangeRateService.CreateRate(rateCreationRequest);
        
        return Results.Ok();
    })
    .WithName("CreateRates")
    .WithOpenApi();

app.Run();