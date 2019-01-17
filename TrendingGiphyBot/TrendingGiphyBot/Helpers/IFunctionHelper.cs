using System.Threading.Tasks;
using TrendingGiphyBot.Containers;

namespace TrendingGiphyBot.Helpers
{
    interface IFunctionHelper
    {
        Task<JobConfigContainer> GetJobConfigAsync(decimal channelId);
        Task<JobConfigContainer> SetJobConfigAsync(decimal channelId, JobConfigContainer jobConfigContainer);
    }
}