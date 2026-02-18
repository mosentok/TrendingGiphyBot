using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using TrendingGiphyBotWorkerService.Configuration;
using TrendingGiphyBotWorkerService.Database;
using TrendingGiphyBotWorkerService.Intervals;

namespace TrendingGiphyBotWorkerService.ChannelSettings;

[RegisterSingleton]
public class ChannelSettingsFinder
(
    ILogger<ChannelSettingsFinder> _logger,
    IServiceScopeFactory _serviceScopeFactory,
    IChannelSettingsFilter _channelSettingsFilter,
    IOptionsMonitor<AppConfig> _appConfig,
    TimeProvider _timeProvider
) : IChannelSettingsFinder
{
    public async Task<List<ulong>> GetChannelSettingsIdsReadyToPostAsync(IEnumerable<ulong> availableChannelIds, CancellationToken stoppingToken)
    {
        var now = _timeProvider.GetUtcNow();
        var validMinutes = _appConfig.CurrentValue.Intervals.Minutes.Where(s => now.Minute % s == 0).ToArray();

        _logger.LogValidMinutes(validMinutes);

        var validHours = now.Minute == 0
            ? _appConfig.CurrentValue.Intervals.Hours.Where(s => now.Hour % s == 0).ToArray()
            : [];

        _logger.LogValidHours(validHours);

        using var scope = _serviceScopeFactory.CreateScope();

        var trendingGiphyBotDbContext = scope.ServiceProvider.GetRequiredService<ITrendingGiphyBotDbContext>();

        var candidateChannelSettings = await trendingGiphyBotDbContext.ChannelSettings
            .Where(channelSettings =>
                availableChannelIds.Contains(channelSettings.ChannelId) &&
                ((channelSettings.Interval == Interval.Minutes && validMinutes.Contains(channelSettings.Frequency)) ||
                (channelSettings.Interval == Interval.Hours && validHours.Contains(channelSettings.Frequency))))
            .ToListAsync(stoppingToken);

        return [.. candidateChannelSettings
            .Where(channelSettings => _channelSettingsFilter.InPostingHours(channelSettings, now))
            .Select(channelSettings => channelSettings.ChannelId)];
    }
}
