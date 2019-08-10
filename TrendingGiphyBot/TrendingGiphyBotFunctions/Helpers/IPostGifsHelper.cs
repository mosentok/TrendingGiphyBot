using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TrendingGiphyBotFunctions.Models;
using TrendingGiphyBotModel;

namespace TrendingGiphyBotFunctions.Helpers
{
    public interface IPostGifsHelper
    {
        Task LogInAsync();
        Task LogOutAsync();
        Task<ChannelResult> BuildChannelContainers(List<UrlHistoryContainer> insertedContainers, ILogger log);
        Task<List<UrlHistoryContainer>> BuildHistoryContainers(List<PendingJobConfig> containers, ILogger log);
        Task DeleteErrorHistories(List<UrlHistoryContainer> errors, ILogger log);
        Task DeleteJobConfigs(List<decimal> channelIds, ILogger log);
        Task<List<PendingJobConfig>> GetContainers(ILogger log);
        Task<List<UrlHistoryContainer>> InsertHistories(List<UrlHistoryContainer> historyContainers, ILogger log);
        Task<GifPostingResult> PostGifs(List<ChannelContainer> channelContainers, ILogger log);
    }
}