using System.Threading.Tasks;
using TrendingGiphyBot.Containers;

namespace TrendingGiphyBot.Helpers
{
    interface IFunctionHelper
    {
        Task<JobConfigContainer> GetJobConfigAsync(decimal channelId);
        Task<JobConfigContainer> PostJobConfigAsync(decimal channelId, JobConfigContainer jobConfigContainer);
        Task<string> GetPrefixAsync(decimal channelId);
        Task<string> PostPrefixAsync(decimal channelId, string prefix);
    }
}