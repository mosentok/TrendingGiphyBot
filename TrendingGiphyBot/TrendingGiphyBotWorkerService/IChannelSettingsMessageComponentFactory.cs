using Discord;

namespace TrendingGiphyBotWorkerService;

public interface IChannelSettingsMessageComponentFactory
{
    MessageComponent BuildChannelSettingsMessageComponent(ChannelSettings channelSettings, string channelName);
}
