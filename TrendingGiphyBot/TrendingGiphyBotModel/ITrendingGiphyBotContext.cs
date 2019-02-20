using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace TrendingGiphyBotModel
{
    public interface ITrendingGiphyBotContext
    {
        DbSet<JobConfig> JobConfigs { get; set; }
        DbSet<UrlCache> UrlCaches { get; set; }
        DbSet<UrlHistory> UrlHistories { get; set; }

        Task DeleteJobConfig(decimal channelId);
        Task<int> DeleteJobConfigs(IEnumerable<decimal> channelIds);
        Task<int> DeleteUrlCachesOlderThan(DateTime oldestDate);
        Task<int> DeleteUrlHistories(List<UrlHistoryContainer> historyContainers);
        Task<int> DeleteUrlHistoriesOlderThan(DateTime oldestDate);
        Task<JobConfigContainer> GetJobConfig(decimal id);
        Task<List<PendingJobConfig>> GetJobConfigsToRun(int nowHour, List<int> currentValidMinutes);
        Task<Dictionary<decimal, string>> GetPrefixDictionary();
        Task<List<UrlCache>> GetUrlCachesAsync();
        Task<int> InsertNewTrendingGifs(List<GifObject> gifObjects);
        Task<List<UrlHistoryContainer>> InsertUrlHistories(List<UrlHistoryContainer> containers);
        Task<JobConfigContainer> SetJobConfig(decimal channelId, JobConfigContainer jobConfigContainer);
    }
}