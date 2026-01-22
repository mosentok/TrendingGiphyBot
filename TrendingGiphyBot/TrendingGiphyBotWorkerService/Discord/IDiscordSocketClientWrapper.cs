using Discord;

namespace TrendingGiphyBotWorkerService.Discord;

public interface IDiscordSocketClientWrapper
{
    ValueTask<IChannel> GetChannelAsync(ulong channelId);
}
