namespace TrendingGiphyBotWorkerService.ChannelSettings;

public interface IChannelSettingsFilter
{
    bool InPostingHours(ChannelSettingsModel channelSettings, DateTimeOffset now);
}