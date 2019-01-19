using System.Threading.Tasks;
using TrendingGiphyBot.Containers;

namespace TrendingGiphyBot.Helpers
{
    public interface IFunctionHelper
    {
        Task<JobConfigContainer> GetJobConfigAsync(decimal channelId);
        Task<JobConfigContainer> PostJobConfigAsync(decimal channelId, JobConfigContainer jobConfigContainer);
        Task DeleteJobConfigAsync(decimal channelId);
        Task<string> GetPrefixAsync(decimal channelId);
        Task<string> PostPrefixAsync(decimal channelId, string prefix);
    }
}