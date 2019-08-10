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
        Task LogInAsync(string botToken);
        Task LogOutAsync();
        int DetermineTotalMinutes(DateTime now);
        List<int> DetermineCurrentValidMinutes(int totalMinutes, List<int> allValidMinutes);
        Task<ChannelResult> BuildChannelContainers(List<UrlHistoryContainer> insertedContainers, ILogger log);
        Task<List<UrlHistoryContainer>> BuildHistoryContainers(List<PendingJobConfig> containers, string giphyRandomEndpoint, ILogger log);
        Task DeleteErrorHistories(List<UrlHistoryContainer> errors, ILogger log);
        Task DeleteJobConfigs(List<decimal> channelIds, ILogger log);
        Task<List<PendingJobConfig>> GetContainers(int nowHour, List<int> currentValidMinutes, ILogger log);
        Task<List<UrlHistoryContainer>> InsertHistories(List<UrlHistoryContainer> historyContainers, ILogger log);
        Task<GifPostingResult> PostGifs(List<ChannelContainer> channelContainers, ILogger log);
    }
}