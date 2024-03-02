using Exchange.Rates.API.Models;
using Exchange.Rates.API.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Exchange.Rates.API.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddLogging();
        services.AddOptions<AlphaVantageSettings>()
            .Configure<IConfiguration>((settings, conf) =>
            conf.GetSection(nameof(AlphaVantageSettings)).Bind(settings));

        services.AddScoped<ExchangeRateService>();
        services.AddHttpClient<AlphaVantageApi>((provider, client) =>
        {
            var settings = provider.GetRequiredService<IOptions<AlphaVantageSettings>>().Value;
            client.BaseAddress = new Uri(settings.BaseUrl ?? throw new InvalidOperationException("AlphaVantage BaseUrl not configured."));
        });

        services.AddDbContext<ApplicationDbContext>(options =>
        options.UseSqlServer(GetConnectionString(configuration)));
        
        return services;
    }

    private static string GetConnectionString(IConfiguration configuration)
    {
        return configuration.GetConnectionString("DefaultConnection")
               ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
    }
}