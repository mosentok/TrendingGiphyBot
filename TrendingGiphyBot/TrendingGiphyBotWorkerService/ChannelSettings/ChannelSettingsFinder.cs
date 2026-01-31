using Microsoft.EntityFrameworkCore;
using TrendingGiphyBotWorkerService.Database;
using TrendingGiphyBotWorkerService.Intervals;

namespace TrendingGiphyBotWorkerService.ChannelSettings;

public class ChannelSettingsFinder
(
    IServiceScopeFactory _serviceScopeFactory,
    IChannelSettingsFilter _channelSettingsFilter,
    IntervalConfig _intervalConfig,
    TimeProvider _timeProvider
) : IChannelSettingsFinder
{
    public async Task<List<ulong>> GetChannelSettingsIdsReadyToPostAsync(IEnumerable<ulong> availableChannelIds, CancellationToken stoppingToken)
    {
        var now = _timeProvider.GetUtcNow();
        var validMinutes = _intervalConfig.Minutes.Where(s => now.Minute % s == 0);

        var validHours = now.Minute == 0
            ? _intervalConfig.Hours.Where(s => now.Hour % s == 0).ToArray()
            : [];

        using var scope = _serviceScopeFactory.CreateScope();

        var trendingGiphyBotDbContext = scope.ServiceProvider.GetRequiredService<ITrendingGiphyBotDbContext>();

        var candidateChannelSettings = await trendingGiphyBotDbContext.ChannelSettings
            .Where(channelSettings =>
                availableChannelIds.Contains(channelSettings.ChannelId) &&
                ((channelSettings.IntervalId == (int)IntervalDescription.Minutes && validMinutes.Contains(channelSettings.Frequency)) ||
                (channelSettings.IntervalId == (int)IntervalDescription.Hours && validHours.Contains(channelSettings.Frequency))))
            .ToListAsync(stoppingToken);

        return [.. candidateChannelSettings
            .Where(channelSettings => _channelSettingsFilter.InPostingHours(channelSettings, now))
            .Select(channelSettings => channelSettings.ChannelId)];
    }
}
