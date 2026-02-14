using Discord;

namespace TrendingGiphyBotWorkerService.Discord.GifPosting.Posting;

public interface IDiscordSocketClientWrapper
{
    ValueTask<IChannel> GetChannelAsync(ulong channelId);
}