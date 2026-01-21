using Discord;

namespace TrendingGiphyBotWorkerService.Discord.Api;

public interface IDiscordSocketClientWrapper
{
    ValueTask<IChannel> GetChannelAsync(ulong channelId);
}
