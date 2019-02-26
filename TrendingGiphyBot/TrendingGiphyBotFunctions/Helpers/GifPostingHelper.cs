using Discord.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrendingGiphyBotFunctions.Models;
using TrendingGiphyBotFunctions.Wrappers;
using TrendingGiphyBotModel;

namespace TrendingGiphyBotFunctions.Helpers
{
    public class GifPostingHelper : IGifPostingHelper
    {
        readonly ILoggerWrapper _Log;
        readonly ITrendingGiphyBotContext _Context;
        readonly IGiphyWrapper _GiphyWrapper;
        readonly IDiscordWrapper _DiscordWrapper;
        public GifPostingHelper(ILoggerWrapper log, ITrendingGiphyBotContext context, IGiphyWrapper giphyWrapper, IDiscordWrapper discordWrapper)
        {
            _Log = log;
            _Context = context;
            _GiphyWrapper = giphyWrapper;
            _DiscordWrapper = discordWrapper;
        }
        public Task LogInAsync() => _DiscordWrapper.LogInAsync();
        public Task LogOutAsync() => _DiscordWrapper.LogOutAsync();
        public int DetermineTotalMinutes(DateTime now)
        {
            if (now.Hour == 0 && now.Minute == 0)
                return 24 * 60;
            return now.Hour * 60 + now.Minute;
        }
        public List<int> DetermineCurrentValidMinutes(int totalMinutes, List<int> allValidMinutes)
        {
            return allValidMinutes.Where(s => totalMinutes % s == 0).ToList();
        }
        public async Task<List<PendingJobConfig>> GetContainers(int nowHour, List<int> currentValidMinutes)
        {
            _Log.LogInformation("Getting job configs.");
            var containers = await _Context.GetJobConfigsToRun(nowHour, currentValidMinutes);
            _Log.LogInformation($"Got {containers.Count} containers.");
            return containers;
        }
        public async Task<List<UrlHistoryContainer>> BuildHistoryContainers(List<PendingJobConfig> containers, string giphyRandomEndpoint)
        {
            _Log.LogInformation($"Building {containers.Count} histories.");
            var urlCaches = await _Context.GetUrlCachesAsync();
            var histories = new List<UrlHistoryContainer>();
            foreach (var container in containers)
            {
                var firstUnseenUrlCache = (from urlCache in urlCaches
                                               //find caches where there are no histories with their gif IDs
                                           where !container.Histories.Any(s => s.GifId == urlCache.Id)
                                           select urlCache).FirstOrDefault();
                if (firstUnseenUrlCache != null)
                    histories.Add(new UrlHistoryContainer(container.ChannelId, firstUnseenUrlCache.Id, firstUnseenUrlCache.Url, true));
                else if (!string.IsNullOrEmpty(container.RandomSearchString))
                {
                    //TODO add a retry loop or something so that we can get/check more than 1 random gif for the channel
                    var randomTagGif = await _GiphyWrapper.GetRandomGifAsync(giphyRandomEndpoint, container.RandomSearchString);
                    //if there are no histories with this random gif's ID
                    if (!container.Histories.Any(s => s.GifId == randomTagGif.Data.Id))
                        histories.Add(new UrlHistoryContainer(container.ChannelId, randomTagGif.Data.Id, randomTagGif.Data.Url, false));
                }
            }
            _Log.LogInformation($"Built {histories.Count} histories.");
            return histories;
        }
        public async Task<List<UrlHistoryContainer>> InsertHistories(List<UrlHistoryContainer> historyContainers)
        {
            _Log.LogInformation($"Inserting {historyContainers.Count} histories.");
            var inserted = await _Context.InsertUrlHistories(historyContainers);
            _Log.LogInformation($"Inserted {inserted.Count} histories.");
            return inserted;
        }
        public async Task<ChannelResult> BuildChannelContainers(List<UrlHistoryContainer> insertedContainers)
        {
            _Log.LogInformation($"Getting {insertedContainers.Count} channels.");
            var channelContainers = new List<ChannelContainer>();
            var errors = new List<UrlHistoryContainer>();
            foreach (var insertedContainer in insertedContainers)
                try
                {
                    var channel = await _DiscordWrapper.GetChannelAsync(insertedContainer.ChannelId);
                    channelContainers.Add(new ChannelContainer(channel, insertedContainer));
                }
                catch (Exception ex)
                {
                    _Log.LogError(ex, $"Error getting channel '{insertedContainer.ChannelId}'.");
                    errors.Add(insertedContainer);
                }
            _Log.LogInformation($"Got {channelContainers.Count} channels.");
            return new ChannelResult(channelContainers, errors);
        }
        public async Task<GifPostingResult> PostGifs(List<ChannelContainer> channelContainers, List<string> warningResponses)
        {
            _Log.LogInformation($"Posting {channelContainers.Count} gifs.");
            var errors = new List<UrlHistoryContainer>();
            var channelsToDelete = new List<UrlHistoryContainer>();
            foreach (var channelContainer in channelContainers)
                try
                {
                    if (channelContainer.Channel != null)
                    {
                        string message;
                        if (channelContainer.HistoryContainer.IsTrending)
                            message = $"*Trending!* {channelContainer.HistoryContainer.Url}";
                        else
                            message = channelContainer.HistoryContainer.Url;
                        await channelContainer.Channel.SendMessageAsync(message);
                    }
                    else
                        channelsToDelete.Add(channelContainer.HistoryContainer);
                }
                catch (HttpException httpException) when (warningResponses.Any(httpException.Message.EndsWith))
                {
                    _Log.LogError(httpException, $"Error posting to channel '{channelContainer.HistoryContainer.ChannelId}' gif '{channelContainer.HistoryContainer.Url}'.");
                    channelsToDelete.Add(channelContainer.HistoryContainer);
                }
                catch (Exception ex)
                {
                    _Log.LogError(ex, $"Error posting to channel '{channelContainer.HistoryContainer.ChannelId}' gif '{channelContainer.HistoryContainer.Url}'.");
                    errors.Add(channelContainer.HistoryContainer);
                }
            var totalCount = channelContainers.Count - errors.Count - channelsToDelete.Count;
            _Log.LogInformation($"Posted {totalCount} gifs.");
            return new GifPostingResult(errors, channelsToDelete);
        }
        public async Task DeleteErrorHistories(List<UrlHistoryContainer> errors)
        {
            _Log.LogError($"Deleting {errors.Count} histories.");
            var deletedCount = await _Context.DeleteUrlHistories(errors);
            _Log.LogError($"Deleted {deletedCount} histories.");
        }
        public async Task DeleteJobConfigs(List<UrlHistoryContainer> doNotExist)
        {
            _Log.LogError($"Deleting {doNotExist.Count} job configs.");
            var channelIds = doNotExist.Select(s => s.ChannelId);
            var deletedCount = await _Context.DeleteJobConfigs(channelIds);
            _Log.LogError($"Deleted {deletedCount} job configs.");
        }
    }
}
