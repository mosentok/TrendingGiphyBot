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
        internal async Task<List<JobConfig>> GetJobConfigsToRun(List<int> curentValidMinutes)
        {
            return await (from jobConfig in JobConfigs
                          where curentValidMinutes.Contains(jobConfig.IntervalMinutes)
                          select jobConfig).ToListAsync();
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
