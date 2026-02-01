using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using TrendingGiphyBotWorkerService.Database;
using TrendingGiphyBotWorkerService.Intervals;
using TrendingGiphyBotWorkerService.Logging;

namespace TrendingGiphyBotWorkerService.ChannelSettings;

public class ChannelSettingsFinder
(
    ILogger<ChannelSettingsFinder> _logger,
    IServiceScopeFactory _serviceScopeFactory,
    IChannelSettingsFilter _channelSettingsFilter,
    IOptions<AppConfig> _appConfig,
    TimeProvider _timeProvider
) : IChannelSettingsFinder
{
    public async Task<List<ulong>> GetChannelSettingsIdsReadyToPostAsync(IEnumerable<ulong> availableChannelIds, CancellationToken stoppingToken)
    {
        var now = _timeProvider.GetUtcNow();
        var validMinutes = _appConfig.Value.Intervals.Minutes.Where(s => now.Minute % s == 0).ToArray();

        _logger.LogValidMinutes(validMinutes);

        var validHours = now.Minute == 0
            ? _appConfig.Value.Intervals.Hours.Where(s => now.Hour % s == 0).ToArray()
            : [];

        _logger.LogValidHours(validHours);

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
