using TrendingGiphyBotWorkerService.ChannelSettings;

namespace TrendingGiphyBotWorkerService.Discord;

public interface IChannelSettingsFilter
{
    bool InPostingHours(ChannelSettingsModel channelSettings, DateTimeOffset now);
}