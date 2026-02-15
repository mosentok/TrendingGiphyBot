using System.Collections.Immutable;
using TrendingGiphyBotWorkerService.Giphy.Staging.GifFinding.Api;

namespace TrendingGiphyBotWorkerService.Discord.GifPosting.Posting;

public interface IGiphyDataChannelPoster
{
    Task PostGiphyGifsAsync(IImmutableDictionary<ulong, GiphyData> stagedChannelGifPosts, Dictionary<ulong, GiphySourceType> channelIdToSourceType, CancellationToken stoppingToken);
}