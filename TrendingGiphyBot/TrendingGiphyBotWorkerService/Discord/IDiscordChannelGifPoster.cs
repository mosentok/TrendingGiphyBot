using System.Collections.Immutable;
using TrendingGiphyBotWorkerService.Giphy.Staging.GifFinding.Api;

namespace TrendingGiphyBotWorkerService.Discord;

public interface IDiscordChannelGifPoster
{
    Task PostGifsAsync(IImmutableDictionary<ulong, GiphyData> stagedChannelGifPosts, List<ulong> channelIds, CancellationToken stoppingToken);
}