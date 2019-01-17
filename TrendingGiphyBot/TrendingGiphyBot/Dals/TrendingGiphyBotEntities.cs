using NLog;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace TrendingGiphyBot.Dals
{
    public partial class TrendingGiphyBotEntities
    {
        static readonly ILogger _Logger = LogManager.GetCurrentClassLogger();
        public TrendingGiphyBotEntities(string nameOrConnectionString) : base(nameOrConnectionString) { }
        internal async Task<bool> AnyChannelConfigs(ulong channelId)
        {
            return await ChannelConfigs.AnyAsync(s => s.ChannelId == channelId);
        }
        internal async Task<string> GetPrefix(ulong channelId)
        {
            return await ChannelConfigs.Where(s => s.ChannelId == channelId).Select(s => s.Prefix).SingleOrDefaultAsync();
        }
        internal async Task SetPrefix(ulong channelId, string prefix)
        {
            var config = await ChannelConfigs.SingleAsync(s => s.ChannelId == channelId);
            config.Prefix = prefix;
            await SaveChangesAsync();
        }
        internal async Task InsertChannelConfig(ulong channelId, string prefix)
        {
            var config = new ChannelConfig { ChannelId = channelId, Prefix = prefix };
            ChannelConfigs.Add(config);
            await SaveChangesAsync();
        }
        internal async Task<bool> AnyJobConfig(decimal id)
        {
            return await JobConfigs.AnyAsync(s => s.ChannelId == id);
        }
        internal async Task<List<decimal>> FindMatchingIds(IEnumerable<decimal> ids)
        {
            var jobConfigIds = JobConfigs.Select(s => s.ChannelId);
            return await jobConfigIds.Intersect(ids).ToListAsync();
        }
        internal async Task<List<JobConfig>> GetJobConfigsToRun(List<int> curentValidMinutes)
        {
            return await (from jobConfig in JobConfigs
                          where curentValidMinutes.Contains(jobConfig.IntervalMinutes)
                          select jobConfig).ToListAsync();
        }
        internal async Task UpdateRandom(ulong channelId, bool randomIsOn, string randomSearchString)
        {
            var match = await JobConfigs.SingleAsync(s => s.ChannelId == channelId);
            match.RandomIsOn = randomIsOn;
            match.RandomSearchString = randomSearchString;
            await SaveChangesAsync();
        }
        internal async Task UpdateQuietHours(ulong channelId, short minHour, short maxHour)
        {
            var match = await JobConfigs.SingleAsync(s => s.ChannelId == channelId);
            match.MinQuietHour = minHour;
            match.MaxQuietHour = maxHour;
            await SaveChangesAsync();
        }
        internal async Task TurnOffRandom(ulong channelId)
        {
            var match = await JobConfigs.SingleAsync(s => s.ChannelId == channelId);
            match.RandomIsOn = false;
            match.RandomSearchString = null;
            await SaveChangesAsync();
        }
        internal async Task TurnOffQuietHours(ulong channelId)
        {
            var match = await JobConfigs.SingleAsync(s => s.ChannelId == channelId);
            match.MinQuietHour = null;
            match.MaxQuietHour = null;
            await SaveChangesAsync();
        }
        internal async Task RemoveJobConfig(decimal channelId)
        {
            var matches = JobConfigs.Where(s => s.ChannelId == channelId);
            JobConfigs.RemoveRange(matches);
            await SaveChangesAsync();
        }
        public async Task BlankRandomConfig(decimal channelId)
        {
            var match = await JobConfigs.SingleAsync(s => s.ChannelId == channelId);
            match.RandomIsOn = false;
            match.RandomSearchString = null;
            await SaveChangesAsync();
        }
        internal async Task InsertUrlCaches(List<string> urls)
        {
            var existingUrls = UrlCaches.Select(s => s.Url);
            var combinedUrls = existingUrls.Concat(urls).Distinct();
            var newUrls = await combinedUrls.Except(existingUrls).ToListAsync();
            var urlCaches = newUrls.Select(s => new UrlCache { Url = s });
            UrlCaches.AddRange(urlCaches);
            await SaveChangesAsync();
        }
        internal async Task DeleteUrlCachesOlderThan(DateTime oldestDate)
        {
            var tooOld = UrlCaches.Where(s => s.Stamp < oldestDate);
            UrlCaches.RemoveRange(tooOld);
            await SaveChangesAsync();
        }
        internal async Task<string> GetLastestUrlNotPosted(decimal channelId)
        {
            var channelsUrls = UrlHistories.Where(s => s.ChannelId == channelId).Select(s => s.Url);
            var latestUrls = UrlCaches.Select(s => s.Url);
            return await latestUrls.Except(channelsUrls).FirstOrDefaultAsync();
        }
        internal async Task<bool> AnyUrlHistories(decimal channelId, string url)
        {
            return await UrlHistories.AnyAsync(s => s.ChannelId == channelId && s.Url == url);
        }
        internal async Task InsertUrlHistories(List<UrlHistory> histories)
        {
            UrlHistories.AddRange(histories);
            await SaveChangesAsync();
        }
        internal async Task DeleteUrlHistoriesOlderThan(DateTime oldestDate)
        {
            var tooOld = UrlHistories.Where(s => s.Stamp < oldestDate);
            UrlHistories.RemoveRange(tooOld);
            await SaveChangesAsync();
        }
    }
}
