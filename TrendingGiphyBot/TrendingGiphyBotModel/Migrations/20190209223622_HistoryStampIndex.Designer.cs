﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TrendingGiphyBotModel;

namespace TrendingGiphyBotModel.Migrations
{
    [DbContext(typeof(TrendingGiphyBotContext))]
    [Migration("20190209223622_HistoryStampIndex")]
    partial class HistoryStampIndex
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.1-servicing-10028")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("TrendingGiphyBotModel.JobConfig", b =>
                {
                    b.Property<decimal>("ChannelId")
                        .HasColumnType("decimal(20,0)");

                    b.Property<short?>("Interval");

                    b.Property<int?>("IntervalMinutes");

                    b.Property<short?>("MaxQuietHour");

                    b.Property<short?>("MinQuietHour");

                    b.Property<string>("Prefix")
                        .HasColumnType("varchar(4)");

                    b.Property<string>("RandomSearchString")
                        .HasColumnType("varchar(32)");

                    b.Property<string>("Time")
                        .HasColumnType("varchar(7)");

                    b.HasKey("ChannelId");

                    b.HasIndex("IntervalMinutes");

                    b.ToTable("JobConfig");
                });

            modelBuilder.Entity("TrendingGiphyBotModel.UrlCache", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("varchar(20)");

                    b.Property<DateTime>("Stamp");

                    b.Property<string>("Url")
                        .HasColumnType("varchar(255)");

                    b.HasKey("Id");

                    b.ToTable("UrlCache");
                });

            modelBuilder.Entity("TrendingGiphyBotModel.UrlHistory", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<decimal>("ChannelId")
                        .HasColumnType("decimal(20,0)");

                    b.Property<string>("GifId")
                        .HasColumnType("varchar(20)");

                    b.Property<DateTime>("Stamp");

                    b.Property<string>("Url")
                        .HasColumnType("varchar(255)");

                    b.HasKey("Id");

                    b.HasIndex("ChannelId");

                    b.HasIndex("GifId");

                    b.HasIndex("Stamp");

                    b.ToTable("UrlHistory");
                });

            modelBuilder.Entity("TrendingGiphyBotModel.UrlHistory", b =>
                {
                    b.HasOne("TrendingGiphyBotModel.JobConfig", "JobConfig")
                        .WithMany("UrlHistories")
                        .HasForeignKey("ChannelId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
