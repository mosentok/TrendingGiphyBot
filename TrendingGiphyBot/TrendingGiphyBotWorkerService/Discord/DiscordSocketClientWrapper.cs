using Discord;
using Discord.WebSocket;

namespace TrendingGiphyBotWorkerService.Discord;

public class DiscordSocketClientWrapper(DiscordSocketClient _discordSocketClient) : IDiscordSocketClientWrapper
{
	public ValueTask<IChannel> GetChannelAsync(ulong channelId) => _discordSocketClient.GetChannelAsync(channelId);
}
