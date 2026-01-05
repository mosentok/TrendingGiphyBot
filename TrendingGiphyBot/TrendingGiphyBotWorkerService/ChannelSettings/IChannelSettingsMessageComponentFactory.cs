using Discord;

namespace TrendingGiphyBotWorkerService.ChannelSettings;

public interface IChannelSettingsMessageComponentFactory
{
    MessageComponent BuildChannelSettingsMessageComponent(ChannelSettingsModel channelSettings, string channelName);
}
