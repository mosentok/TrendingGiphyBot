namespace TrendingGiphyBotWorkerService.ChannelSettings;

public interface IChannelSettingsFinder
{
    Task<List<ulong>> GetChannelSettingsIdsReadyToPostAsync(IEnumerable<ulong> availableChannelIds, CancellationToken stoppingToken);
}
