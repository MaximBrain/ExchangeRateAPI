using Exchange.Rates.API.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Exchange.Rates.API.Services;

public interface IRepository
{
    Task SaveChangesAsync();
    Task<CurrencyExchangeRate> AddToDatabase(CurrencyExchangeRate exchangeRate);
    Task<CurrencyExchangeRate?> GetCurrentExchangeRate(string from, string to);
    Task DeleteRate(CurrencyExchangeRate current);
    Currency AddCurrencyIfNotExists(Currency newToCurrency);
}

public record RatesRepository(ApplicationDbContext ApplicationDbContext): IRepository
{
    public async Task SaveChangesAsync()
    {
        await ApplicationDbContext.SaveChangesAsync();
    }

    public async Task<CurrencyExchangeRate> AddToDatabase(CurrencyExchangeRate exchangeRate)
    {
        ApplicationDbContext.ExchangeRates.Add(exchangeRate);
        await SaveChangesAsync();
        return exchangeRate;
    }

    public async Task<CurrencyExchangeRate?> GetCurrentExchangeRate(string from, string to)
    {
        var exchangeRates = await ApplicationDbContext.ExchangeRates
            .FirstOrDefaultAsync(d => d.FromCurrency!.CurrencyCode.Equals(from, StringComparison.InvariantCultureIgnoreCase)
                                      && d.ToCurrency!.CurrencyCode.Equals(to, StringComparison.InvariantCultureIgnoreCase));
        return exchangeRates;
    }

    public async Task DeleteRate(CurrencyExchangeRate current)
    {
        ApplicationDbContext.Set<CurrencyExchangeRate>().Remove(current);
        await SaveChangesAsync();
    }

    public Currency AddCurrencyIfNotExists(Currency newToCurrency)
    {
        return ApplicationDbContext.Set<Currency>().AddIfNotExists(newToCurrency,
            x => x.CurrencyCode == newToCurrency.CurrencyCode);
    }
}