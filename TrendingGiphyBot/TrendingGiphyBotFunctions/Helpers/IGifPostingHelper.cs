using System;
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
        int DetermineTotalMinutes(DateTime now);
        List<int> DetermineCurrentValidMinutes(int totalMinutes, List<int> allValidMinutes);
        Task<ChannelResult> BuildChannelContainers(List<UrlHistoryContainer> insertedContainers);
        Task<List<UrlHistoryContainer>> BuildHistoryContainers(List<PendingJobConfig> containers, string giphyRandomEndpoint);
        Task DeleteErrorHistories(List<UrlHistoryContainer> errors);
        Task DeleteJobConfigs(List<decimal> channelIds);
        Task<List<PendingJobConfig>> GetContainers(int nowHour, List<int> currentValidMinutes);
        Task<List<UrlHistoryContainer>> InsertHistories(List<UrlHistoryContainer> historyContainers);
        Task<GifPostingResult> PostGifs(List<ChannelContainer> channelContainers);
    }
}