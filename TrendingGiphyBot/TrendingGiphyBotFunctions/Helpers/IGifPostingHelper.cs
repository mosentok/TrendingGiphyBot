using System.Collections.Generic;
using System.Threading.Tasks;
using TrendingGiphyBotFunctions.Models;
using TrendingGiphyBotModel;

namespace TrendingGiphyBotFunctions.Helpers
{
    public interface IGifPostingHelper
    {
        Task LogInAsync();
        Task LogOutAsync();
        Task<ChannelResult> BuildChannelContainers(List<UrlHistoryContainer> insertedContainers);
        Task<List<UrlHistoryContainer>> BuildHistoryContainers(List<PendingJobConfig> containers);
        Task DeleteErrorHistories(List<UrlHistoryContainer> errors);
        Task DeleteJobConfigs(List<UrlHistoryContainer> doNotExist);
        Task<List<PendingJobConfig>> GetContainers();
        Task<List<UrlHistoryContainer>> InsertHistories(List<UrlHistoryContainer> historyContainers);
        Task<GifPostingResult> PostGifs(List<ChannelContainer> channelContainers);
    }
}