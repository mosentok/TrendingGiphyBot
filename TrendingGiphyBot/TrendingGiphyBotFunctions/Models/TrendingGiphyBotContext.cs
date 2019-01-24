using System;
using System.Collections.Generic;
using System.Data;
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

        public virtual DbSet<ChannelConfig> ChannelConfigs { get; set; }
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
            return await Database.ExecuteSqlCommandAsync($"DELETE FROM UrlCache WHERE Stamp < {oldestDate}");
        }
        public async Task<int> DeleteUrlHistoriesOlderThan(DateTime oldestDate)
        {
            return await Database.ExecuteSqlCommandAsync($"DELETE FROM UrlHistory WHERE Stamp < {oldestDate}");
        }
        public async Task<int> DeleteUrlHistories(List<UrlHistoryContainer> historyContainers)
        {
            var histories = historyContainers.Select(s => new UrlHistory { ChannelId = s.ChannelId, Url = s.Url }).ToList();
            UrlHistories.AttachRange(histories);
            UrlHistories.RemoveRange(histories);
            return await SaveChangesAsync();
        }
        public async Task<int> DeleteJobConfigs(IEnumerable<decimal> channelIds)
        {
            var jobConfigs = channelIds.Select(s => new JobConfig { ChannelId = s }).ToList();
            JobConfigs.AttachRange(jobConfigs);
            JobConfigs.RemoveRange(jobConfigs);
            return await SaveChangesAsync();
        }
        public async Task<List<UrlHistoryContainer>> InsertUrlHistories(List<UrlHistoryContainer> containers)
        {
            var trendingGifs = containers.Where(s => s.IsTrending).ToList();
            var randomGifs = containers.Except(trendingGifs);
            var randomGifsNotInHistory = (from randomGif in randomGifs
                                          join history in UrlHistories on randomGif.ChannelId equals history.ChannelId into histories
                                          where !histories.Select(s => s.Url).Contains(randomGif.Url)
                                          select randomGif).ToList();
            var toInsert = trendingGifs.Concat(randomGifsNotInHistory).ToList();
            using (var table = new DataTable())
            using (var bulkCopy = new SqlBulkCopy(_ConnectionString))
            {
                table.Columns.Add(nameof(UrlHistory.ChannelId));
                table.Columns.Add(nameof(UrlHistory.Url));
                foreach (var container in toInsert)
                {
                    var row = table.NewRow();
                    row[nameof(UrlHistory.ChannelId)] = container.ChannelId;
                    row[nameof(UrlHistory.Url)] = container.Url;
                    table.Rows.Add(row);
                }
                bulkCopy.DestinationTableName = nameof(UrlHistory);
                await bulkCopy.WriteToServerAsync(table);
            }
            return toInsert;
        }
        public async Task<int> InsertNewTrendingGifs(List<GifObject> gifObjects)
        {
            var existingGifObjects = from gifObject in gifObjects
                                     join urlCache in UrlCaches on gifObject.Url equals urlCache.Url
                                     select gifObject;
            var newGifObjects = gifObjects.Except(existingGifObjects);
            var newUrlCaches = newGifObjects.Select(s => new UrlCache { Url = s.Url, Stamp = DateTime.Now });
            UrlCaches.AddRange(newUrlCaches);
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
                RandomSearchString = s.RandomSearchString,
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
                    ChannelId = channelId,
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
                RandomSearchString = jobConfig.RandomSearchString,
                MinQuietHour = jobConfig.MinQuietHour,
                MaxQuietHour = jobConfig.MaxQuietHour,
            };
        }
        public async Task DeleteJobConfig(decimal channelId)
        {
            await Database.ExecuteSqlCommandAsync($"DELETE FROM JobConfig WHERE ChannelId = {channelId}");
        }
        public async Task<string> GetPrefix(decimal channelId)
        {
            return await ChannelConfigs.Where(s => s.ChannelId == channelId).Select(s => s.Prefix).SingleOrDefaultAsync();
        }
        public async Task<string> SetPrefix(decimal channelId, string prefix)
        {
            ChannelConfig channelConfig;
            var match = await ChannelConfigs.SingleOrDefaultAsync(s => s.ChannelId == channelId);
            if (match != null)
            {
                channelConfig = match;
                match.Prefix = prefix;
            }
            else
            {
                channelConfig = new ChannelConfig { ChannelId = channelId, Prefix = prefix };
                ChannelConfigs.Add(channelConfig);
            }
            await SaveChangesAsync();
            return channelConfig.Prefix;
        }
        public async Task<List<PendingContainer>> GetJobConfigsToRun(int nowHour, List<int> currentValidMinutes)
        {
            var latestUrls = UrlCaches.Select(s => s.Url);
            return await (from jobConfig in JobConfigs
                          let minPostingHour = jobConfig.MaxQuietHour
                          let maxPostingHour = jobConfig.MinQuietHour
                          where currentValidMinutes.Contains(jobConfig.IntervalMinutes) && //where config's interval minutes are valid, and...
                            (minPostingHour == null || maxPostingHour == null || //either no limit on posting, or there are posting hour limits to check.
                            (minPostingHour < maxPostingHour && nowHour >= minPostingHour && nowHour < maxPostingHour) || //if the range spans inside a single day, then now hour must be between min and max, else...
                            (minPostingHour > maxPostingHour && (nowHour >= minPostingHour || nowHour < maxPostingHour))) //if the range spans across two days, then now hour must be between overnight hours
                          join urlHistory in UrlHistories on jobConfig.ChannelId equals urlHistory.ChannelId into histories
                          let firstUnseenUrl = latestUrls.Except(histories.Select(s => s.Url)).FirstOrDefault()
                          where (!string.IsNullOrEmpty(firstUnseenUrl) || //where either we found a fresh gif from the cache, or...
                            jobConfig.RandomIsOn) //random is on, so they'll want a random gif instead
                          select new PendingContainer
                          {
                              ChannelId = jobConfig.ChannelId,
                              FirstUnseenUrl = firstUnseenUrl,
                              RandomSearchString = jobConfig.RandomSearchString
                          }).ToListAsync();
        }
        static int DetermineIntervalMinutes(JobConfigContainer jobConfigContainer)
        {
            if (jobConfigContainer.Time == "Hour" || jobConfigContainer.Time == "Hours")
                return jobConfigContainer.Interval * 60;
            return jobConfigContainer.Interval;
        }
    }
}
