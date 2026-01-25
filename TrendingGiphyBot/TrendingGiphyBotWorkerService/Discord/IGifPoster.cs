using System.Collections.Immutable;
using TrendingGiphyBotWorkerService.Giphy.Api;

namespace TrendingGiphyBotWorkerService.Discord;

public interface IGifPoster
{
    Task PostGifsAsync(IImmutableDictionary<ulong, GiphyData> stagedChannelGifPosts, List<ulong> channelIds, CancellationToken stoppingToken);
}