using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using TrendingGiphyBotModel.Extensions;

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
            //TODO this doesn't work because we need to either search on the ID that bulk copy created, or manually search by channel ID and gif ID
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
            await SaveChangesAsync();
            return jobConfigs.Count;
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
            //TODO speed this up
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
            var match = await (from jobConfig in JobConfigs
                               where jobConfig.ChannelId == id
                               select new JobConfigContainer
                               {
                                   ChannelId = jobConfig.ChannelId,
                                   Interval = jobConfig.Interval,
                                   Time = jobConfig.Time,
                                   RandomSearchString = jobConfig.RandomSearchString,
                                   MinQuietHour = jobConfig.MinQuietHour,
                                   MaxQuietHour = jobConfig.MaxQuietHour,
                                   Filters = jobConfig.GifFilters.Select(t => t.Filter).ToList()
                               }).SingleOrDefaultAsync();
            if (match != null)
                return match;
            return new JobConfigContainer { ChannelId = id };
        }
        public async Task<JobConfigContainer> SetJobConfig(decimal channelId, JobConfigContainer jobConfigContainer)
        {
            var gifFilters = jobConfigContainer.Filters.Select(s => new GifFilter { ChannelId = channelId, Filter = s }).ToList();
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
                match.GifFilters = gifFilters;
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
                    GifFilters = gifFilters
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
                Filters = jobConfig.GifFilters.Select(s => s.Filter).ToList()
            };
        }
        public async Task DeleteJobConfig(decimal channelId)
        {
            await Database.ExecuteSqlCommandAsync($"DELETE FROM JobConfig WHERE ChannelId = {channelId}");
        }
        public async Task<List<PendingContainer>> GetJobConfigsToRun(int nowHour, List<int> currentValidMinutes)
        {
            var urlCaches = await UrlCaches.AsNoTracking().ToListAsync();
            var containers = await (from jobConfig in JobConfigs
                                    where jobConfig.IntervalMinutes.HasValue && currentValidMinutes.Contains(jobConfig.IntervalMinutes.Value) && //where config's interval minutes are valid, and...
                                      (jobConfig.MaxQuietHour == null || jobConfig.MinQuietHour == null || //either no limit on posting, or there are posting hour limits to check.
                                      (jobConfig.MaxQuietHour < jobConfig.MinQuietHour && nowHour >= jobConfig.MaxQuietHour && nowHour < jobConfig.MinQuietHour) || //if the range spans inside a single day, then now hour must be between min and max, else...
                                      (jobConfig.MaxQuietHour > jobConfig.MinQuietHour && (nowHour >= jobConfig.MaxQuietHour || nowHour < jobConfig.MinQuietHour))) //if the range spans across two days, then now hour must be between overnight hours
                                    select new
                                    {
                                        ChannelId = jobConfig.ChannelId,
                                        RandomSearchString = jobConfig.RandomSearchString,
                                        HistoryGifIds = (from history in jobConfig.UrlHistories
                                                         select history.GifId).ToList(),
                                        Filters = (from filter in jobConfig.GifFilters
                                                   select filter.Filter).ToList()
                                    }).AsNoTracking().ToListAsync();
            return (from container in containers
                    select new PendingContainer
                    {
                        ChannelId = container.ChannelId,
                        RandomSearchString = container.RandomSearchString,
                        FirstUnseenUrlCache = (from urlCache in urlCaches
                                               where !urlCache.Url.ContainsAnyFilter(container.Filters) &&
                                                     !container.HistoryGifIds.Contains(urlCache.Id)
                                               select urlCache).FirstOrDefault()
                    }).ToList();
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
