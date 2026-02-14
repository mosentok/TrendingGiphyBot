using System.Collections.Immutable;
using TrendingGiphyBotWorkerService.Giphy.Staging.GifFinding.Api;

namespace TrendingGiphyBotWorkerService.Giphy.Staging;

public interface IGiphyDataStage
{
    IImmutableDictionary<ulong, GiphyData> GetTrendingGiphyPostStage();
    IImmutableDictionary<ulong, GiphyData> GetSearchGiphyPostStage();
    IImmutableDictionary<ulong, GiphyData> GetRandomGiphyPostStage();
    Task RefreshAsync(CancellationToken cancellationToken = default);
    void EvictTrending(ulong channelId);
    void EvictSearch(ulong channelId);
    void EvictRandom(ulong channelId);
}
