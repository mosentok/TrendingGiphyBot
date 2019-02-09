using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TrendingGiphyBotMigrator
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var basePath = AppContext.BaseDirectory;
            var builder = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables();
            var config = builder.Build();
            var originalConnectionString = config.GetConnectionString("OriginalConnectionString");
            var originalBuilder = new DbContextOptionsBuilder<OriginalModel.TrendingGiphyBotContext>()
                .UseSqlServer(originalConnectionString);
            using (var originalContext = new OriginalModel.TrendingGiphyBotContext(originalBuilder.Options))
            {
                var originalContainers = from jobConfig in originalContext.JobConfig
                                         join urlHistory in originalContext.UrlHistory on jobConfig.ChannelId equals urlHistory.ChannelId into urlHistories
                                         join channelConfig in originalContext.ChannelConfig on jobConfig.ChannelId equals channelConfig.ChannelId into channelConfigs
                                         select new OriginalJobConfigContainer
                                         {
                                             JobConfig = jobConfig,
                                             Histories = urlHistories,
                                             ChannelConfig = channelConfigs.DefaultIfEmpty().FirstOrDefault()
                                         };
                var newJobConfigs = await originalContainers.Select(s =>
                    new TrendingGiphyBotModel.JobConfig
                    {
                        ChannelId = s.JobConfig.ChannelId,
                        Interval = (short)s.JobConfig.Interval,
                        IntervalMinutes = s.JobConfig.IntervalMinutes,
                        MaxQuietHour = s.JobConfig.MaxQuietHour,
                        MinQuietHour = s.JobConfig.MinQuietHour,
                        Prefix = s.ChannelConfig != null ? s.ChannelConfig.Prefix : null,
                        RandomSearchString = s.JobConfig.RandomSearchString,
                        Time = s.JobConfig.Time,
                        UrlHistories = s.Histories.Select(t => new TrendingGiphyBotModel.UrlHistory
                        {
                            ChannelId = t.ChannelId,
                            Stamp = t.Stamp,
                            Url = t.Url
                        }).ToList()
                    }).ToListAsync();
                foreach (var jobConfig in newJobConfigs)
                    foreach (var urlHistory in jobConfig.UrlHistories)
                        urlHistory.GifId = urlHistory.Url.Split('/', '-').Last();
                var anyBlankIds = newJobConfigs.SelectMany(s => s.UrlHistories).Any(s => string.IsNullOrEmpty(s.GifId));
                if (anyBlankIds)
                    throw new Exception("found some blank IDs, aborting");
                var newConnectionString = config.GetConnectionString("NewConnectionString");
                var newBuilder = new DbContextOptionsBuilder<TrendingGiphyBotModel.TrendingGiphyBotContext>()
                    .UseSqlServer(newConnectionString);
                using (var newContext = new TrendingGiphyBotModel.TrendingGiphyBotContext(newBuilder.Options))
                {
                    await newContext.InsertJobConfigs(newJobConfigs);
                    /*
                     * we can't tell based on the original data whether the history was trending or not
                     * if we migrate the existing histories with IsTrending = true, then new not-trending results will never get checked against the migrated records, mean duplicates are possible, causing an error
                     * so we should set IsTrending = false
                     */
                    var histories = newJobConfigs.SelectMany(s => s.UrlHistories).Select(s => new TrendingGiphyBotModel.UrlHistoryContainer { ChannelId = s.ChannelId, GifId = s.GifId, IsTrending = false, Url = s.Url }).ToList();
                    await newContext.InsertUrlHistories(histories);
                }
            }
        }
    }
    public class OriginalJobConfigContainer
    {
        public OriginalModel.JobConfig JobConfig { get; set; }
        public IEnumerable<OriginalModel.UrlHistory> Histories { get; set; }
        public OriginalModel.ChannelConfig ChannelConfig { get; set; }
    }
    public class NewJobConfigContainer
    {
        public TrendingGiphyBotModel.JobConfig JobConfig { get; set; }
        public IEnumerable<TrendingGiphyBotModel.UrlHistory> Histories { get; set; }
    }
}
