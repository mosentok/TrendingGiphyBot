using Microsoft.EntityFrameworkCore;
using TrendingGiphyBotWorkerService.Database;
using TrendingGiphyBotWorkerService.Delaying;
using TrendingGiphyBotWorkerService.Giphy.Staging;
using TrendingGiphyBotWorkerService.Intervals;

namespace TrendingGiphyBotWorkerService.Discord;

public class DiscordPostingWorker(
    IServiceScopeFactory _serviceScopeFactory,
    IGifPostStage _gifPostStage,
    IDelayer _delayer,
    IGifPoster _gifPoster,
    IntervalConfig _intervalConfig,
    TimeProvider _timeProvider
) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await _delayer.DelayUntilNextPostingTimeAsync(stoppingToken);

            var now = _timeProvider.GetUtcNow();
            var validMinutes = _intervalConfig.Minutes.Where(s => now.Minute % s == 0);

            var validHours = now.Minute == 0
                ? _intervalConfig.Hours.Where(s => now.Hour % s == 0).ToArray()
                : [];

            var stagedChannelGifPosts = _gifPostStage.GetChannelGifPostStage();

            using var scope = _serviceScopeFactory.CreateScope();

            var trendingGiphyBotDbContext = scope.ServiceProvider.GetRequiredService<ITrendingGiphyBotDbContext>();

            var activeChannelIds = await trendingGiphyBotDbContext.ChannelSettings
                .Where(channelSettings =>
                    stagedChannelGifPosts.Keys.Contains(channelSettings.ChannelId) &&
                    ((channelSettings.IntervalId == (int)IntervalDescription.Minutes && validMinutes.Contains(channelSettings.Frequency)) ||
                    (channelSettings.IntervalId == (int)IntervalDescription.Hours && validHours.Contains(channelSettings.Frequency))))
                .Select(s => s.ChannelId)
                .ToListAsync(stoppingToken);

            await _gifPoster.PostGifsAsync(stagedChannelGifPosts, activeChannelIds, stoppingToken);
        }
    }
}
