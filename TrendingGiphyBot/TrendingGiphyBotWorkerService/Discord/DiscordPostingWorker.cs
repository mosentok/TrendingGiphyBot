using Microsoft.EntityFrameworkCore;
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
    IDelayer _delayer,
    IGifPoster _gifPoster,
    IChannelSettingsFilter _channelSettingsFilter,
    IntervalConfig _intervalConfig,
    TimeProvider _timeProvider
) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await _delayer.DelayUntilNextPostingTimeAsync(stoppingToken);

            _logger.LogPostingGifs();

            var now = _timeProvider.GetUtcNow();
            var validMinutes = _intervalConfig.Minutes.Where(s => now.Minute % s == 0);

            var validHours = now.Minute == 0
                ? _intervalConfig.Hours.Where(s => now.Hour % s == 0).ToArray()
                : [];

            var stagedChannelGifPosts = _gifPostStage.GetChannelGifPostStage();

            using var scope = _serviceScopeFactory.CreateScope();

            var trendingGiphyBotDbContext = scope.ServiceProvider.GetRequiredService<ITrendingGiphyBotDbContext>();

            var candidateChannelSettings = await trendingGiphyBotDbContext.ChannelSettings
                .Where(channelSettings =>
                    stagedChannelGifPosts.Keys.Contains(channelSettings.ChannelId) &&
                    ((channelSettings.IntervalId == (int)IntervalDescription.Minutes && validMinutes.Contains(channelSettings.Frequency)) ||
                    (channelSettings.IntervalId == (int)IntervalDescription.Hours && validHours.Contains(channelSettings.Frequency))))
                .ToListAsync(stoppingToken);

            var channelIdsInPostingHours = candidateChannelSettings
                .Where(channelSettings => _channelSettingsFilter.InPostingHours(channelSettings, now))
                .Select(channelSettings => channelSettings.ChannelId)
                .ToList();

            await _gifPoster.PostGifsAsync(stagedChannelGifPosts, channelIdsInPostingHours, stoppingToken);

            _logger.LogPostedGifs();
        }
    }
}
