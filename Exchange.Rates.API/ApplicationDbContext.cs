using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Exchange.Rates.API;

[Index(nameof(CurrencyCode))]
public class Currency
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int CurrencyId { get; set; }

    [Required]
    [Column(TypeName = "nvarchar(3)")]
    public string CurrencyCode { get; set; }
    public string? CurrencyName { get; set; }
}

[Index(nameof(FromCurrencyId), nameof(ToCurrencyId))]
public class CurrencyExchangeRate
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int ExchangeRateId { get; set; }

    public int FromCurrencyId { get; set; }

    [ForeignKey("FromCurrencyId")] public Currency? FromCurrency { get; set; }

    public int ToCurrencyId { get; set; }

    [ForeignKey("ToCurrencyId")] public Currency? ToCurrency { get; set; }

    [Column(TypeName = "decimal(18,8)")] public decimal ExchangeRate { get; set; }

    [Column(TypeName = "decimal(18,8)")] public decimal BidPrice { get; set; }

    [Column(TypeName = "decimal(18,8)")] public decimal AskPrice { get; set; }

    public DateTime EffectiveDate { get; set; }
}

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Currency> Currencies { get; set; }

    public DbSet<CurrencyExchangeRate> ExchangeRates { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Currency>().HasIndex(c => c.CurrencyCode).IsUnique();
        
        modelBuilder.Entity<CurrencyExchangeRate>().HasIndex(c => new { c.FromCurrencyId, c.ToCurrencyId }).IsUnique();
        
        modelBuilder.Entity<CurrencyExchangeRate>().HasOne(c => c.FromCurrency)
            .WithMany()
            .HasForeignKey(c => c.FromCurrencyId)
            .OnDelete(DeleteBehavior.NoAction);
        
        modelBuilder.Entity<CurrencyExchangeRate>().HasOne(c => c.ToCurrency)
            .WithMany()
            .HasForeignKey(c => c.ToCurrencyId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}