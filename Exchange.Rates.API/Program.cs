using Exchange.Rates.API.Extensions;
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

app.UseHttpsRedirection();

app.MapGet("/rates/{from}/{to}",
        async (ExchangeRateService exchangeRateService, string from, string to)  =>
    {
        var rate = await exchangeRateService.GetRate(from, to);
        
        // convert the rate to a string with fancy formatting
        var formattedRate = rate.ToString("0.00##");
        return Results.Ok($"1 {from} = {formattedRate} {to}");
    })
    .WithName("GetRates")
    .WithOpenApi();

app.MapPost(
        "/rates/{from}/{to}/{amount:decimal}",
        async (ExchangeRateService exchangeRateService, string from, string to, decimal amount) =>
    {
        // Create Rates with ExchangeRateService help and return OK
        await exchangeRateService.CreateRate(from, to, amount);
        return Results.Ok();
    })
    .WithName("CreateRates")
    .WithOpenApi();

app.Run();