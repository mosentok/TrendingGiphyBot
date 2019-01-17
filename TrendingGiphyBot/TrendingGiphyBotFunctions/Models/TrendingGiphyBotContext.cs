using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace TrendingGiphyBotFunctions.Models
{
    public partial class TrendingGiphyBotContext : DbContext
    {
        readonly string _ConnectionString;
        public TrendingGiphyBotContext()
        {
        }

        public TrendingGiphyBotContext(DbContextOptions<TrendingGiphyBotContext> options)
            : base(options)
        {
        }

        public TrendingGiphyBotContext(string connectionString)
        {
            _ConnectionString = connectionString;
        }

        public virtual DbSet<ChannelConfig> ChannelConfig { get; set; }
        public virtual DbSet<JobConfig> JobConfigs { get; set; }
        public virtual DbSet<Time> Time { get; set; }
        public virtual DbSet<UrlCache> UrlCaches { get; set; }
        public virtual DbSet<UrlHistory> UrlHistories { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(_ConnectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            foreach (var entity in modelBuilder.Model.GetEntityTypes())
                entity.Relational().TableName = entity.DisplayName();
            modelBuilder.Entity<ChannelConfig>(entity =>
            {
                entity.HasKey(e => e.ChannelId);

                entity.Property(e => e.ChannelId).HasColumnType("numeric(20, 0)");

                entity.Property(e => e.Prefix)
                    .IsRequired()
                    .HasMaxLength(4)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<JobConfig>(entity =>
            {
                entity.HasKey(e => e.ChannelId);

                entity.Property(e => e.ChannelId).HasColumnType("numeric(20, 0)");

                entity.Property(e => e.RandomSearchString)
                    .HasMaxLength(32)
                    .IsUnicode(false);

                entity.Property(e => e.Time)
                    .IsRequired()
                    .HasMaxLength(16)
                    .IsUnicode(false);

                entity.HasOne(d => d.TimeNavigation)
                    .WithMany(p => p.JobConfig)
                    .HasForeignKey(d => d.Time)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_JobConfig_Time");
            });

            modelBuilder.Entity<Time>(entity =>
            {
                entity.HasKey(e => e.Value);

                entity.Property(e => e.Value)
                    .HasMaxLength(16)
                    .IsUnicode(false)
                    .ValueGeneratedNever();
            });

            modelBuilder.Entity<UrlCache>(entity =>
            {
                entity.HasKey(e => e.Url);

                entity.Property(e => e.Url)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .ValueGeneratedNever();

                entity.Property(e => e.Stamp)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");
            });

            modelBuilder.Entity<UrlHistory>(entity =>
            {
                entity.HasKey(e => new { e.ChannelId, e.Url });

                entity.HasIndex(e => e.Stamp)
                    .HasName("nci_wi_UrlHistory_7886CC02C4D996621453061A88A8026B");

                entity.Property(e => e.ChannelId).HasColumnType("numeric(20, 0)");

                entity.Property(e => e.Url)
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.Stamp)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");
            });
        }
        public async Task<int> DeleteUrlCachesOlderThan(DateTime oldestDate)
        {
            var sql = "DELETE FROM UrlCache WHERE Stamp < @oldestDate";
            var parameter = new SqlParameter("@oldestDate", oldestDate);
            return await Database.ExecuteSqlCommandAsync(sql, parameter);
        }
        public async Task<int> DeleteUrlHistoriesOlderThan(DateTime oldestDate)
        {
            var sql = "DELETE FROM UrlHistory WHERE Stamp < @oldestDate";
            var parameter = new SqlParameter("@oldestDate", oldestDate);
            return await Database.ExecuteSqlCommandAsync(sql, parameter);
        }
        public async Task<int> InsertNewTrendingGifs(List<GifObject> gifObjects)
        {
            var existingGifObjects = from gifObject in gifObjects
                                     join urlCache in UrlCaches on gifObject.Url equals urlCache.Url
                                     select gifObject;
            var newGifObjects = gifObjects.Except(existingGifObjects);
            var newUrlCaches = newGifObjects.Select(s => new UrlCache { Url = s.Url, Stamp = DateTime.Now });
            await UrlCaches.AddRangeAsync(newUrlCaches);
            return await SaveChangesAsync();
        }
        public async Task<JobConfigContainer> GetJobConfig(decimal id)
        {
            return await JobConfigs.Where(s => s.ChannelId == id).Select(s => new JobConfigContainer
            {
                ChannelId = s.ChannelId,
                Interval = s.Interval,
                Time = s.Time,
                RandomIsOn = s.RandomIsOn,
                MinQuietHour = s.MinQuietHour,
                MaxQuietHour = s.MaxQuietHour,
            }).SingleOrDefaultAsync();
        }
        public async Task<JobConfigContainer> SetJobConfig(decimal channelId, JobConfigContainer jobConfigContainer)
        {
            JobConfig jobConfig;
            var match = await JobConfigs.SingleOrDefaultAsync(s => s.ChannelId == channelId);
            if (match != null) //already exists, so mutate it
            {
                jobConfig = match;
                match.Interval = jobConfigContainer.Interval;
                match.Time = jobConfigContainer.Time;
                match.IntervalMinutes = DetermineIntervalMinutes(jobConfigContainer);
                match.RandomIsOn = jobConfigContainer.RandomIsOn;
                match.RandomSearchString = jobConfigContainer.RandomSearchString;
                match.MinQuietHour = jobConfigContainer.MinQuietHour;
                match.MaxQuietHour = jobConfigContainer.MaxQuietHour;
            }
            else //new item
            {
                jobConfig = new JobConfig
                {
                    Interval = jobConfigContainer.Interval,
                    Time = jobConfigContainer.Time,
                    IntervalMinutes = DetermineIntervalMinutes(jobConfigContainer),
                    RandomIsOn = jobConfigContainer.RandomIsOn,
                    RandomSearchString = jobConfigContainer.RandomSearchString,
                    MinQuietHour = jobConfigContainer.MinQuietHour,
                    MaxQuietHour = jobConfigContainer.MaxQuietHour
                };
                JobConfigs.Add(jobConfig);
            }
            await SaveChangesAsync();
            return new JobConfigContainer
            {
                ChannelId = jobConfig.ChannelId,
                Interval = jobConfig.Interval,
                Time = jobConfig.Time,
                RandomIsOn = jobConfig.RandomIsOn,
                MinQuietHour = jobConfig.MinQuietHour,
                MaxQuietHour = jobConfig.MaxQuietHour,
            };
        }
        static int DetermineIntervalMinutes(JobConfigContainer jobConfigContainer)
        {
            if (jobConfigContainer.Time == "Hour" || jobConfigContainer.Time == "Hours")
                return jobConfigContainer.Interval * 60;
            return jobConfigContainer.Interval;
        }
    }
}
