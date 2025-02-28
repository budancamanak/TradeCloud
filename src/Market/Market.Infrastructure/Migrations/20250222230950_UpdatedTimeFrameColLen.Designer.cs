﻿// <auto-generated />
using System;
using Market.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Market.Infrastructure.Migrations
{
    [DbContext(typeof(MarketDbContext))]
    [Migration("20250222230950_UpdatedTimeFrameColLen")]
    partial class UpdatedTimeFrameColLen
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.1")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Market.Domain.Entities.Exchange", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("ConnectionUrl")
                        .HasMaxLength(200)
                        .HasColumnType("character varying(200)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)");

                    b.HasKey("Id");

                    b.ToTable("Exchanges", (string)null);
                });

            modelBuilder.Entity("Market.Domain.Entities.Price", b =>
                {
                    b.Property<DateTime>("Timestamp")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("TickerId")
                        .HasColumnType("integer");

                    b.Property<string>("Timeframe")
                        .HasMaxLength(3)
                        .HasColumnType("character varying(3)");

                    b.Property<double>("Close")
                        .HasColumnType("double precision");

                    b.Property<double>("High")
                        .HasColumnType("double precision");

                    b.Property<double>("Low")
                        .HasColumnType("double precision");

                    b.Property<double>("Open")
                        .HasColumnType("double precision");

                    b.HasKey("Timestamp", "TickerId", "Timeframe");

                    b.HasIndex("TickerId");

                    b.ToTable("Prices", (string)null);
                });

            modelBuilder.Entity("Market.Domain.Entities.Ticker", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("DecimalPoint")
                        .HasColumnType("integer");

                    b.Property<int>("ExchangeId")
                        .HasColumnType("integer");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)");

                    b.Property<string>("Symbol")
                        .IsRequired()
                        .HasMaxLength(25)
                        .HasColumnType("character varying(25)");

                    b.HasKey("Id");

                    b.HasIndex("ExchangeId");

                    b.ToTable("Tickers", (string)null);
                });

            modelBuilder.Entity("Market.Domain.Entities.Price", b =>
                {
                    b.HasOne("Market.Domain.Entities.Ticker", "Ticker")
                        .WithMany("Prices")
                        .HasForeignKey("TickerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Ticker");
                });

            modelBuilder.Entity("Market.Domain.Entities.Ticker", b =>
                {
                    b.HasOne("Market.Domain.Entities.Exchange", "Exchange")
                        .WithMany("Tickers")
                        .HasForeignKey("ExchangeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Exchange");
                });

            modelBuilder.Entity("Market.Domain.Entities.Exchange", b =>
                {
                    b.Navigation("Tickers");
                });

            modelBuilder.Entity("Market.Domain.Entities.Ticker", b =>
                {
                    b.Navigation("Prices");
                });
#pragma warning restore 612, 618
        }
    }
}
