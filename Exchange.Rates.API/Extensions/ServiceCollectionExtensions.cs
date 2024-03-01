using Exchange.Rates.API.Models;
using Exchange.Rates.API.Services;
using Microsoft.Extensions.Options;

namespace Exchange.Rates.API.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddOptions<AlphaVantageSettings>()
            .Configure<IConfiguration>((settings, configuration) =>
            configuration.GetSection(nameof(AlphaVantageSettings)).Bind(settings));

        services.AddScoped<ExchangeRateService>();
        services.AddHttpClient<AlphaVantageApi>((provider, client) =>
        {
            var settings = provider.GetRequiredService<IOptions<AlphaVantageSettings>>().Value;
            client.BaseAddress = new Uri(settings.BaseUrl ?? throw new InvalidOperationException("AlphaVantage BaseUrl not configured."));
        });
        
        return services;
    }
}