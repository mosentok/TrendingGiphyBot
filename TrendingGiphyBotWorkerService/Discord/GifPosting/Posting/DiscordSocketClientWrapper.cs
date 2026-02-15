using Discord;
using Discord.WebSocket;

namespace TrendingGiphyBotWorkerService.Discord.GifPosting.Posting;

[RegisterSingleton]
public class DiscordSocketClientWrapper(
    DiscordSocketClient _discordSocketClient
) : IDiscordSocketClientWrapper
{
    public ValueTask<IChannel> GetChannelAsync(ulong channelId) => _discordSocketClient.GetChannelAsync(channelId);
}