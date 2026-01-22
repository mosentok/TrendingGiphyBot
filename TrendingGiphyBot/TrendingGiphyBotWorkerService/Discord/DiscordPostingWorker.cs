using Discord;
using Microsoft.EntityFrameworkCore;
using TrendingGiphyBotWorkerService.ChannelSettings;
using TrendingGiphyBotWorkerService.Database;
using TrendingGiphyBotWorkerService.Delaying;
using TrendingGiphyBotWorkerService.Giphy.Staging;
using TrendingGiphyBotWorkerService.Intervals;
using TrendingGiphyBotWorkerService.Logging;

namespace TrendingGiphyBotWorkerService.Discord;

public class DiscordPostingWorker(
    ILogger<DiscordPostingWorker> _logger,
    IServiceScopeFactory _serviceScopeFactory,
    IGifPostStage _gifPostStage,
    IDiscordSocketClientWrapper _discordSocketClientWrapper,
    IDelayer _delayer,
    IntervalConfig _intervalConfig,
    TimeProvider _timeProvider
) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await _delayer.DelayUntilNextPostingTimeAsync(stoppingToken);

            var stagedChannelGifPosts = _gifPostStage.GetChannelGifPostStage();
            var now = _timeProvider.GetUtcNow();
            var validMinutes = _intervalConfig.Minutes.Where(s => now.Minute % s == 0);

            var validHours = now.Minute == 0
                ? _intervalConfig.Hours.Where(s => now.Hour % s == 0).ToArray()
                : [];

            using var scope = _serviceScopeFactory.CreateScope();

            var trendingGiphyBotDbContext = scope.ServiceProvider.GetRequiredService<ITrendingGiphyBotDbContext>();

            var activeChannelIds = await trendingGiphyBotDbContext.ChannelSettings
                .Where(channelSettings =>
                    stagedChannelGifPosts.Keys.Contains(channelSettings.ChannelId) &&
                    ((channelSettings.IntervalId == (int)IntervalDescription.Minutes && validMinutes.Contains(channelSettings.Frequency)) ||
                    (channelSettings.IntervalId == (int)IntervalDescription.Hours && validHours.Contains(channelSettings.Frequency))))
                .Select(s => s.ChannelId)
                .ToListAsync(stoppingToken);

            // TODO parallelize this loop?
            foreach (var channelId in activeChannelIds)
                try
                {
                    var channel = await _discordSocketClientWrapper.GetChannelAsync(channelId);

                    if (channel is not IMessageChannel messageChannel)
                        throw new ThisShouldBeImpossibleException();

                    var gifPost = new GifPost { ChannelId = channelId, GiphyDataId = stagedChannelGifPosts[channelId].Id };

                    trendingGiphyBotDbContext.GifPosts.Add(gifPost);

                    await trendingGiphyBotDbContext.SaveChangesAsync(stoppingToken);

                    try
                    {
                        await messageChannel.SendMessageAsync($"*Trending!* {stagedChannelGifPosts[channelId].Url}");

                        _gifPostStage.Evict(channelId);
                    }
                    catch (Exception postingException)
                    {
                        var gifPostId = gifPost.GifPostId;

                        // TODO [LoggerMessage]
                        _logger.LogError(postingException, "An exception caught when posting GiphyDataId {GiphyDataId} to ChannelId {ChannelId}. Removing GifPostId {GifPostId}.", stagedChannelGifPosts[channelId].Id, channelId, gifPostId);

                        trendingGiphyBotDbContext.GifPosts.Remove(gifPost);

                        await trendingGiphyBotDbContext.SaveChangesAsync(stoppingToken);

                        _logger.LogDebug("Removed GifPostId {GifPostId}.", gifPostId);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogGifPostingException(ex);
                }
        }
    }
}
