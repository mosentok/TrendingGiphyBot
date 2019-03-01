using System.Collections.Generic;
using System.Threading.Tasks;
using TrendingGiphyBotModel;

namespace TrendingGiphyBotCore.Wrappers
{
    public interface IFunctionWrapper
    {
        Task<JobConfigContainer> GetJobConfigAsync(decimal channelId);
        Task<JobConfigContainer> PostJobConfigAsync(decimal channelId, JobConfigContainer jobConfigContainer);
        Task DeleteJobConfigAsync(decimal channelId);
        Task PostStatsAsync(ulong botId, int guildCount);
        Task<Dictionary<decimal, string>> GetPrefixDictionaryAsync();
    }
}