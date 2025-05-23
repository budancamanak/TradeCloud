﻿// <auto-generated />
using System;
using Backend.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Backend.Infrastructure.Migrations
{
    [DbContext(typeof(BackendDbContext))]
    [Migration("20250422233254_AnalysisExecutionProgressChanged")]
    partial class AnalysisExecutionProgressChanged
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.1")
                .HasAnnotation("Proxies:ChangeTracking", false)
                .HasAnnotation("Proxies:CheckEquality", false)
                .HasAnnotation("Proxies:LazyLoading", true)
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Backend.Domain.Entities.AnalysisExecution", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime>("EndDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("ParamSet")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("PluginIdentifier")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)");

                    b.Property<int>("ProgressCurrent")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasDefaultValue(0);

                    b.Property<int>("ProgressTotal")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasDefaultValue(0);

                    b.Property<DateTime>("StartDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("TickerId")
                        .HasColumnType("integer");

                    b.Property<string>("Timeframe")
                        .IsRequired()
                        .HasMaxLength(3)
                        .HasColumnType("character varying(3)");

                    b.Property<string>("TradingParams")
                        .HasColumnType("text");

                    b.Property<int>("UserId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.ToTable("AnalysisExecutions", (string)null);
                });

            modelBuilder.Entity("Backend.Domain.Entities.PluginExecution", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("AnalysisExecutionId")
                        .HasColumnType("integer");

                    b.Property<string>("Error")
                        .HasColumnType("text");

                    b.Property<DateTime?>("FinishDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("ParamSet")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<double>("Progress")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("double precision")
                        .HasDefaultValue(0.0);

                    b.Property<DateTime?>("QueuedDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime?>("RunStartDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("character varying(20)");

                    b.HasKey("Id");

                    b.HasIndex("AnalysisExecutionId");

                    b.ToTable("PluginExecutions", (string)null);
                });

            modelBuilder.Entity("Backend.Domain.Entities.PluginOutput", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("PluginId")
                        .HasColumnType("integer");

                    b.Property<string>("PluginSignal")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("character varying(20)");

                    b.Property<DateTime>("SignalDate")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.HasIndex("PluginId");

                    b.ToTable("PluginOutputs", (string)null);
                });

            modelBuilder.Entity("Backend.Domain.Entities.SystemSetting", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Setting")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Value")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("SystemSettings", (string)null);
                });

            modelBuilder.Entity("Backend.Domain.Entities.TrackList", b =>
                {
                    b.Property<int>("TickerId")
                        .HasColumnType("integer");

                    b.Property<int>("UserId")
                        .HasColumnType("integer");

                    b.HasKey("TickerId", "UserId");

                    b.ToTable("UserTrackLists", (string)null);
                });

            modelBuilder.Entity("Backend.Domain.Entities.PluginExecution", b =>
                {
                    b.HasOne("Backend.Domain.Entities.AnalysisExecution", "AnalysisExecution")
                        .WithMany("PluginExecutions")
                        .HasForeignKey("AnalysisExecutionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("AnalysisExecution");
                });

            modelBuilder.Entity("Backend.Domain.Entities.PluginOutput", b =>
                {
                    b.HasOne("Backend.Domain.Entities.PluginExecution", "PluginExecution")
                        .WithMany("PluginOutputs")
                        .HasForeignKey("PluginId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("PluginExecution");
                });

            modelBuilder.Entity("Backend.Domain.Entities.AnalysisExecution", b =>
                {
                    b.Navigation("PluginExecutions");
                });

            modelBuilder.Entity("Backend.Domain.Entities.PluginExecution", b =>
                {
                    b.Navigation("PluginOutputs");
                });
#pragma warning restore 612, 618
        }
    }
}
