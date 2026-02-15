using System.Collections.Immutable;
using TrendingGiphyBotWorkerService.Discord.GifPosting;

namespace TrendingGiphyBotWorkerService.Discord;

public interface IGiphyDataChannelPoster
{
    Task PostGiphyGifsAsync(IImmutableDictionary<ulong, GiphyGifPostSelection> selections, CancellationToken stoppingToken);
}