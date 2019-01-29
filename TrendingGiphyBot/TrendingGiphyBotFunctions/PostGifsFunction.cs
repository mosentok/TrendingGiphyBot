using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Net;
using Discord.Rest;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using TrendingGiphyBotFunctions.Helpers;
using TrendingGiphyBotFunctions.Models;
using TrendingGiphyBotModel;

namespace TrendingGiphyBotFunctions
{
    public class PostGifsFunction : IDisposable
    {
        [FunctionName(nameof(PostGifsFunction))]
        public static async Task Run([TimerTrigger("%PostGifsFunctionCron%")]TimerInfo myTimer, ILogger log)
        {
            var connectionString = Environment.GetEnvironmentVariable("TrendingGiphyBotConnectionString");
            using (var postGifsFunction = new PostGifsFunction(new TrendingGiphyBotContext(connectionString), new GiphyHelper(), new DiscordRestClient(), new TaskCompletionSource<bool>(), new TaskCompletionSource<bool>(), log))
                await postGifsFunction.RunAsync();
        }
        readonly TrendingGiphyBotContext _Context;
        readonly GiphyHelper _GiphyHelper;
        readonly DiscordRestClient _DiscordClient;
        readonly TaskCompletionSource<bool> _LoggedInSource;
        readonly TaskCompletionSource<bool> _LoggedOutSource;
        readonly ILogger _Log;
        public PostGifsFunction(TrendingGiphyBotContext Context, GiphyHelper GiphyHelper, DiscordRestClient Client, TaskCompletionSource<bool> LoggedInSource, TaskCompletionSource<bool> LoggedOutSource, ILogger Log)
        {
            _Context = Context;
            _GiphyHelper = GiphyHelper;
            _DiscordClient = Client;
            _LoggedInSource = LoggedInSource;
            _LoggedOutSource = LoggedOutSource;
            _Log = Log;
        }
        public async Task RunAsync()
        {
            var connectionString = Environment.GetEnvironmentVariable("TrendingGiphyBotConnectionString");
            _DiscordClient.LoggedIn += LoggedIn;
            var token = Environment.GetEnvironmentVariable("BotToken");
            await _DiscordClient.LoginAsync(TokenType.Bot, token);
            await _LoggedInSource.Task;
            var pendingContainers = await GetContainers();
            var historyContainers = await BuildHistoryContainers(pendingContainers);
            var insertedContainers = await InsertHistories(historyContainers);
            var gifPostingResult = await PostGifs(insertedContainers);
            if (gifPostingResult.Errors.Any())
                await DeleteErrorHistories(gifPostingResult.Errors);
            if (gifPostingResult.ChannelsToDelete.Any())
                await DeleteJobConfigs(gifPostingResult.ChannelsToDelete);
            _DiscordClient.LoggedOut += LoggedOut;
            await _DiscordClient.LogoutAsync();
            await _LoggedOutSource.Task;
            _DiscordClient.LoggedIn -= LoggedIn;
            _DiscordClient.LoggedOut -= LoggedOut;
        }
        async Task<List<UrlHistoryContainer>> InsertHistories(List<UrlHistoryContainer> historyContainers)
        {
            _Log.LogInformation($"Inserting {historyContainers.Count} histories.");
            var inserted = await _Context.InsertUrlHistories(historyContainers);
            _Log.LogInformation($"Inserted {inserted.Count} histories.");
            return inserted;
        }
        Task LoggedIn()
        {
            _LoggedInSource.SetResult(true);
            return Task.CompletedTask;
        }
        async Task<List<PendingContainer>> GetContainers()
        {
            _Log.LogInformation("Getting job configs.");
            var hourOffsetString = Environment.GetEnvironmentVariable("HourOffset");
            var hourOffset = int.Parse(hourOffsetString);
            var now = DateTime.Now.AddHours(-hourOffset);
            var validMinutesString = Environment.GetEnvironmentVariable("ValidMinutes");
            var validMinutes = validMinutesString.Split(',').Select(int.Parse);
            var validHoursString = Environment.GetEnvironmentVariable("ValidHours");
            var validHours = validHoursString.Split(',').Select(int.Parse);
            var validHoursAsMinutes = validHours.Select(s => s * 60);
            var allValidMinutes = validMinutes.Concat(validHoursAsMinutes);
            int totalMinutes;
            if (now.Hour == 0)
                totalMinutes = 24 * 60;
            else
                totalMinutes = now.Hour * 60 + now.Minute;
            var currentValidMinutes = allValidMinutes.Where(s => totalMinutes % s == 0).ToList();
            var containers = await _Context.GetJobConfigsToRun(now.Hour, currentValidMinutes);
            _Log.LogInformation($"Got {containers.Count} containers.");
            return containers;
        }
        async Task<List<UrlHistoryContainer>> BuildHistoryContainers(List<PendingContainer> containers)
        {
            _Log.LogInformation($"Building {containers.Count} histories.");
            var giphyRandomEndpoint = Environment.GetEnvironmentVariable("GiphyRandomEndpoint");
            var randomGif = await _GiphyHelper.GetRandomGif(giphyRandomEndpoint);
            var histories = new List<UrlHistoryContainer>();
            foreach (var container in containers)
                if (!string.IsNullOrEmpty(container.FirstUnseenUrl))
                    histories.Add(new UrlHistoryContainer(container.ChannelId, container.FirstUnseenGifId, container.FirstUnseenUrl, true));
                else if (!string.IsNullOrEmpty(container.RandomSearchString))
                {
                    var randomTagGif = await _GiphyHelper.GetRandomGif(giphyRandomEndpoint, container.RandomSearchString);
                    histories.Add(new UrlHistoryContainer(container.ChannelId, container.FirstUnseenGifId, randomTagGif.Data.Url, false));
                }
                else
                    histories.Add(new UrlHistoryContainer(container.ChannelId, container.FirstUnseenGifId, randomGif.Data.Url, false));
            _Log.LogInformation($"Built {histories.Count} histories.");
            return histories;
        }
        async Task DeleteErrorHistories(List<UrlHistoryContainer> errors)
        {
            _Log.LogError($"Deleting {errors.Count} histories.");
            var deletedCount = await _Context.DeleteUrlHistories(errors);
            _Log.LogError($"Deleted {deletedCount} histories.");
        }
        async Task DeleteJobConfigs(List<UrlHistoryContainer> doNotExist)
        {
            _Log.LogError($"Deleting {doNotExist.Count} job configs.");
            var channelIds = doNotExist.Select(s => s.ChannelId);
            var deletedCount = await _Context.DeleteJobConfigs(channelIds);
            _Log.LogError($"Deleted {deletedCount} job configs.");
        }
        async Task<GifPostingResult> PostGifs(List<UrlHistoryContainer> historyContainers)
        {
            _Log.LogInformation($"Posting {historyContainers.Count} gifs.");
            var errors = new List<UrlHistoryContainer>();
            var channelsToDelete = new List<UrlHistoryContainer>();
            foreach (var historyContainer in historyContainers)
                try
                {
                    var channelId = Convert.ToUInt64(historyContainer.ChannelId);
                    var channel = await _DiscordClient.GetChannelAsync(channelId) as IMessageChannel;
                    if (channel != null)
                    {
                        string message;
                        if (historyContainer.IsTrending)
                            message = $"*Trending!* {historyContainer.Url}";
                        else
                            message = historyContainer.Url;
                        await channel.SendMessageAsync(message);
                    }
                    else
                        channelsToDelete.Add(historyContainer);
                }
                catch (HttpException httpException) when (httpException.Message.EndsWith("Missing Access") ||
                                                          httpException.Message.EndsWith("Missing Permissions"))
                {
                    _Log.LogError(httpException, $"Error posting to channel '{historyContainer.ChannelId}' gif '{historyContainer.Url}'.");
                    channelsToDelete.Add(historyContainer);
                }
                catch (Exception ex)
                {
                    _Log.LogError(ex, $"Error posting to channel '{historyContainer.ChannelId}' gif '{historyContainer.Url}'.");
                    errors.Add(historyContainer);
                }
            var totalCount = historyContainers.Count - errors.Count - channelsToDelete.Count;
            _Log.LogInformation($"Posted {totalCount} gifs.");
            return new GifPostingResult(errors, channelsToDelete);
        }
        Task LoggedOut()
        {
            _LoggedOutSource.SetResult(true);
            return Task.CompletedTask;
        }
        public void Dispose()
        {
            _DiscordClient?.Dispose();
            _Context?.Dispose();
            _GiphyHelper?.Dispose();
        }
    }
}
