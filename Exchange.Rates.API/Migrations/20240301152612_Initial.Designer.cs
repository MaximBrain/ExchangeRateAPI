﻿// <auto-generated />
using System;
using Exchange.Rates.API;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Exchange.Rates.API.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20240301152612_Initial")]
    partial class Initial
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.2")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Exchange.Rates.API.Currency", b =>
                {
                    b.Property<int>("CurrencyId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("CurrencyId"));

                    b.Property<string>("CurrencyCode")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("CurrencyName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("CurrencyId");

                    b.HasIndex("CurrencyCode")
                        .IsUnique();

                    b.ToTable("Currencies");
                });

            modelBuilder.Entity("Exchange.Rates.API.CurrencyExchangeRate", b =>
                {
                    b.Property<int>("ExchangeRateId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ExchangeRateId"));

                    b.Property<decimal>("AskPrice")
                        .HasColumnType("decimal(18,8)");

                    b.Property<decimal>("BidPrice")
                        .HasColumnType("decimal(18,8)");

                    b.Property<DateTime>("EffectiveDate")
                        .HasColumnType("datetime2");

                    b.Property<decimal>("ExchangeRate")
                        .HasColumnType("decimal(18,8)");

                    b.Property<int>("FromCurrencyId")
                        .HasColumnType("int");

                    b.Property<int>("ToCurrencyId")
                        .HasColumnType("int");

                    b.HasKey("ExchangeRateId");

                    b.HasIndex("ToCurrencyId");

                    b.HasIndex("FromCurrencyId", "ToCurrencyId")
                        .IsUnique();

                    b.ToTable("ExchangeRates");
                });

            modelBuilder.Entity("Exchange.Rates.API.CurrencyExchangeRate", b =>
                {
                    b.HasOne("Exchange.Rates.API.Currency", "FromCurrency")
                        .WithMany()
                        .HasForeignKey("FromCurrencyId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("Exchange.Rates.API.Currency", "ToCurrency")
                        .WithMany()
                        .HasForeignKey("ToCurrencyId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("FromCurrency");

                    b.Navigation("ToCurrency");
                });
#pragma warning restore 612, 618
        }
    }
}
