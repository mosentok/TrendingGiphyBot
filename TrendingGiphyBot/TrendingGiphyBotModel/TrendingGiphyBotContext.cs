﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace TrendingGiphyBotModel
{
    public partial class TrendingGiphyBotContext : DbContext, ITrendingGiphyBotContext
    {
        public TrendingGiphyBotContext() { }
        public TrendingGiphyBotContext(DbContextOptions<TrendingGiphyBotContext> options) : base(options) { }
        //TODO remove hardcoded command timeout
        public TrendingGiphyBotContext(string connectionString) : this(new DbContextOptionsBuilder<TrendingGiphyBotContext>()
            .UseSqlServer(connectionString, options => options.CommandTimeout((int)TimeSpan.FromMinutes(10).TotalSeconds))
            .UseLazyLoadingProxies().Options)
        { }
        public virtual DbSet<JobConfig> JobConfigs { get; set; }
        public virtual DbSet<UrlCache> UrlCaches { get; set; }
        public virtual DbSet<UrlHistory> UrlHistories { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            foreach (var entity in modelBuilder.Model.GetEntityTypes())
                entity.Relational().TableName = entity.DisplayName();
            modelBuilder.Entity<JobConfig>()
                .HasIndex(s => s.IntervalMinutes);
            modelBuilder.Entity<UrlHistory>()
                .HasIndex(s => s.GifId);
            modelBuilder.Entity<UrlHistory>()
                .HasIndex(s => s.Stamp);
            modelBuilder.Entity<UrlHistory>()
                .ForSqlServerHasIndex(s => s.ChannelId)
                .ForSqlServerInclude(s => s.GifId);
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
            var historiesToDelete = await (from urlHistory in UrlHistories
                                           //where there are any containers matching the channel ID and gif ID
                                           where (from container in historyContainers
                                                  where urlHistory.ChannelId == container.ChannelId &&
                                                        urlHistory.GifId == container.GifId
                                                  select container).Any()
                                           select urlHistory).ToListAsync();
            UrlHistories.RemoveRange(historiesToDelete);
            await SaveChangesAsync();
            return historiesToDelete.Count;
        }
        public async Task<int> DeleteJobConfigs(IEnumerable<decimal> channelIds)
        {
            var jobConfigs = channelIds.Select(s => new JobConfig { ChannelId = s }).ToList();
            JobConfigs.AttachRange(jobConfigs);
            JobConfigs.RemoveRange(jobConfigs);
            await SaveChangesAsync();
            return jobConfigs.Count;
        }
        public async Task<List<UrlHistoryContainer>> InsertUrlHistories(List<UrlHistoryContainer> containers)
        {
            var connectionString = Database.GetDbConnection().ConnectionString;
            using (var table = new DataTable())
            //TODO remove hardcoded command timeout
            using (var bulkCopy = new SqlBulkCopy(connectionString) { BulkCopyTimeout = 600 })
            {
                table.Columns.Add(nameof(UrlHistory.Id));
                table.Columns.Add(nameof(UrlHistory.ChannelId));
                table.Columns.Add(nameof(UrlHistory.GifId));
                table.Columns.Add(nameof(UrlHistory.Url));
                table.Columns.Add(nameof(UrlHistory.Stamp));
                foreach (var container in containers)
                {
                    var row = table.NewRow();
                    row[nameof(UrlHistory.ChannelId)] = container.ChannelId;
                    row[nameof(UrlHistory.GifId)] = container.GifId;
                    row[nameof(UrlHistory.Url)] = container.Url;
                    row[nameof(UrlHistory.Stamp)] = DateTime.Now;
                    table.Rows.Add(row);
                }
                bulkCopy.DestinationTableName = nameof(UrlHistory);
                await bulkCopy.WriteToServerAsync(table);
            }
            return containers;
        }
        public async Task<int> InsertNewTrendingGifs(List<GifObject> gifObjects)
        {
            var existingGifObjects = from gifObject in gifObjects
                                     join urlCache in UrlCaches on gifObject.Id equals urlCache.Id
                                     select gifObject;
            var newGifObjects = gifObjects.Except(existingGifObjects);
            var newUrlCaches = newGifObjects.Select(s => new UrlCache { Id = s.Id, Url = s.Url, Stamp = DateTime.Now });
            UrlCaches.AddRange(newUrlCaches);
            return await SaveChangesAsync();
        }
        public async Task<JobConfigContainer> GetJobConfig(decimal id)
        {
            var jobConfig = await JobConfigs.Where(s => s.ChannelId == id).Select(s => new JobConfigContainer
            {
                ChannelId = s.ChannelId,
                Interval = s.Interval,
                Time = s.Time,
                RandomSearchString = s.RandomSearchString,
                MinQuietHour = s.MinQuietHour,
                MaxQuietHour = s.MaxQuietHour,
                Prefix = s.Prefix
            }).SingleOrDefaultAsync();
            if (jobConfig != null)
                return jobConfig;
            return new JobConfigContainer { ChannelId = id };
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
                match.RandomSearchString = jobConfigContainer.RandomSearchString;
                match.MinQuietHour = jobConfigContainer.MinQuietHour;
                match.MaxQuietHour = jobConfigContainer.MaxQuietHour;
                match.Prefix = jobConfigContainer.Prefix;
            }
            else //new item
            {
                jobConfig = new JobConfig
                {
                    ChannelId = channelId,
                    Interval = jobConfigContainer.Interval,
                    Time = jobConfigContainer.Time,
                    IntervalMinutes = DetermineIntervalMinutes(jobConfigContainer),
                    RandomSearchString = jobConfigContainer.RandomSearchString,
                    MinQuietHour = jobConfigContainer.MinQuietHour,
                    MaxQuietHour = jobConfigContainer.MaxQuietHour,
                    Prefix = jobConfigContainer.Prefix
                };
                JobConfigs.Add(jobConfig);
            }
            await SaveChangesAsync();
            return new JobConfigContainer
            {
                ChannelId = jobConfig.ChannelId,
                Interval = jobConfig.Interval,
                Time = jobConfig.Time,
                RandomSearchString = jobConfig.RandomSearchString,
                MinQuietHour = jobConfig.MinQuietHour,
                MaxQuietHour = jobConfig.MaxQuietHour,
                Prefix = jobConfig.Prefix
            };
        }
        public async Task DeleteJobConfig(decimal channelId)
        {
            await Database.ExecuteSqlCommandAsync($"DELETE FROM JobConfig WHERE ChannelId = {channelId}");
        }
        public async Task<List<PendingJobConfig>> GetJobConfigsToRun(int nowHour, List<int> currentValidMinutes)
        {
            return await (from jobConfig in JobConfigs
                          where jobConfig.IntervalMinutes.HasValue && currentValidMinutes.Contains(jobConfig.IntervalMinutes.Value) && //where config's interval minutes are valid, and...
                               (jobConfig.MaxQuietHour == null || jobConfig.MinQuietHour == null || //either no limit on posting, or there are posting hour limits to check.
                               (jobConfig.MaxQuietHour < jobConfig.MinQuietHour && nowHour >= jobConfig.MaxQuietHour && nowHour < jobConfig.MinQuietHour) || //if the range spans inside a single day, then now hour must be between min and max, else...
                               (jobConfig.MaxQuietHour > jobConfig.MinQuietHour && (nowHour >= jobConfig.MaxQuietHour || nowHour < jobConfig.MinQuietHour))) //if the range spans across two days, then now hour must be between overnight hours
                          select new PendingJobConfig
                          {
                              ChannelId = jobConfig.ChannelId,
                              RandomSearchString = jobConfig.RandomSearchString,
                              Histories = (from history in jobConfig.UrlHistories
                                           select new PendingHistory
                                           {
                                               GifId = history.GifId
                                           }).ToList()
                          }).AsNoTracking().ToListAsync();
        }
        public async Task<List<UrlCache>> GetUrlCachesAsync()
        {
            return await UrlCaches.AsNoTracking().ToListAsync();
        }
        public async Task<Dictionary<decimal, string>> GetPrefixDictionary()
        {
            return await (from jobConfig in JobConfigs
                          where !string.IsNullOrEmpty(jobConfig.Prefix)
                          select jobConfig).ToDictionaryAsync(s => s.ChannelId, s => s.Prefix);
        }
        static int? DetermineIntervalMinutes(JobConfigContainer jobConfigContainer)
        {
            if (!jobConfigContainer.Interval.HasValue || string.IsNullOrEmpty(jobConfigContainer.Time))
                return null;
            if (jobConfigContainer.Time == "Hour" || jobConfigContainer.Time == "Hours")
                return jobConfigContainer.Interval.Value * 60;
            return jobConfigContainer.Interval.Value;
        }
    }
}
