using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace TrendingGiphyBotModel
{
    public partial class TrendingGiphyBotContext : DbContext
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
        //TODO remove this when migrator is removed
        public async Task<List<JobConfig>> InsertJobConfigs(List<JobConfig> toInsert)
        {
            //need to make sure none of the random gifs we retrieved are already in the database
            var connectionString = Database.GetDbConnection().ConnectionString;
            using (var table = new DataTable())
            using (var bulkCopy = new SqlBulkCopy(connectionString))
            {
                table.Columns.Add(nameof(JobConfig.ChannelId));
                table.Columns.Add(nameof(JobConfig.Interval));
                table.Columns.Add(nameof(JobConfig.Time));
                table.Columns.Add(nameof(JobConfig.RandomSearchString));
                table.Columns.Add(nameof(JobConfig.MinQuietHour));
                table.Columns.Add(nameof(JobConfig.MaxQuietHour));
                table.Columns.Add(nameof(JobConfig.IntervalMinutes));
                table.Columns.Add(nameof(JobConfig.Prefix));
                foreach (var container in toInsert)
                {
                    var row = table.NewRow();
                    row[nameof(JobConfig.ChannelId)] = container.ChannelId;
                    row[nameof(JobConfig.Interval)] = container.Interval;
                    row[nameof(JobConfig.Time)] = container.Time;
                    row[nameof(JobConfig.RandomSearchString)] = container.RandomSearchString;
                    row[nameof(JobConfig.MinQuietHour)] = container.MinQuietHour;
                    row[nameof(JobConfig.MaxQuietHour)] = container.MaxQuietHour;
                    row[nameof(JobConfig.IntervalMinutes)] = container.IntervalMinutes;
                    row[nameof(JobConfig.Prefix)] = container.Prefix;
                    table.Rows.Add(row);
                }
                bulkCopy.DestinationTableName = nameof(JobConfig);
                await bulkCopy.WriteToServerAsync(table);
            }
            return toInsert;
        }
        public async Task<List<UrlHistoryContainer>> InsertUrlHistories(List<UrlHistoryContainer> containers)
        {
            //need to make sure none of the random gifs we retrieved are already in the database
            var toInsert = RemoveDuplicateHistories(containers);
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
                foreach (var container in toInsert)
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
            return toInsert;
        }
        List<UrlHistoryContainer> RemoveDuplicateHistories(List<UrlHistoryContainer> containers)
        {
            var trendingGifs = containers.Where(s => s.IsTrending).ToList();
            var randomGifs = containers.Except(trendingGifs);
            var randomGifsNotInHistory = (from randomGif in randomGifs
                                          join history in UrlHistories on randomGif.ChannelId equals history.ChannelId into histories
                                          where !histories.Select(s => s.Url).Contains(randomGif.Url)
                                          select randomGif).ToList();
            return trendingGifs.Concat(randomGifsNotInHistory).ToList();
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
                //TODO uncomment this
                //match.Prefix = jobConfigContainer.Prefix;
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
                    //TODO uncomment this
                    //Prefix = jobConfigContainer.Prefix
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
                //TODO uncomment this
                //Prefix = jobConfig.Prefix
            };
        }
        public async Task DeleteJobConfig(decimal channelId)
        {
            await Database.ExecuteSqlCommandAsync($"DELETE FROM JobConfig WHERE ChannelId = {channelId}");
        }
        public async Task<List<PendingContainer>> GetJobConfigsToRun(int nowHour, List<int> currentValidMinutes)
        {
            return await (from jobConfig in JobConfigs
                          let minPostingHour = jobConfig.MaxQuietHour
                          let maxPostingHour = jobConfig.MinQuietHour
                          where jobConfig.IntervalMinutes.HasValue && currentValidMinutes.Contains(jobConfig.IntervalMinutes.Value) && //where config's interval minutes are valid, and...
                            (minPostingHour == null || maxPostingHour == null || //either no limit on posting, or there are posting hour limits to check.
                            (minPostingHour < maxPostingHour && nowHour >= minPostingHour && nowHour < maxPostingHour) || //if the range spans inside a single day, then now hour must be between min and max, else...
                            (minPostingHour > maxPostingHour && (nowHour >= minPostingHour || nowHour < maxPostingHour))) //if the range spans across two days, then now hour must be between overnight hours
                          let alreadySeenGifIds = jobConfig.UrlHistories.Select(s => s.GifId)
                          let firstUnseenUrlCache = UrlCaches.Where(s => !alreadySeenGifIds.Contains(s.Id)).FirstOrDefault()
                          where firstUnseenUrlCache != null || //where either we found a fresh gif from the cache, or...
                            !string.IsNullOrEmpty(jobConfig.RandomSearchString) //random is on, so they'll want a random gif instead
                          select new PendingContainer
                          {
                              ChannelId = jobConfig.ChannelId,
                              FirstUnseenGifId = firstUnseenUrlCache.Id,
                              FirstUnseenUrl = firstUnseenUrlCache.Url,
                              RandomSearchString = jobConfig.RandomSearchString
                          }).ToListAsync();
        }
        public async Task<string> GetPrefix(decimal channelId)
        {
            return await JobConfigs.Where(s => s.ChannelId == channelId).Select(s => s.Prefix).SingleOrDefaultAsync();
        }
        public async Task<string> SetPrefix(decimal channelId, string prefix)
        {
            var jobConfig = await JobConfigs.Where(s => s.ChannelId == channelId).SingleOrDefaultAsync();
            if (jobConfig == null)
                return null;
            if (!string.IsNullOrEmpty(prefix))
                jobConfig.Prefix = prefix;
            else
                jobConfig.Prefix = null;
            await SaveChangesAsync();
            return jobConfig.Prefix;
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
