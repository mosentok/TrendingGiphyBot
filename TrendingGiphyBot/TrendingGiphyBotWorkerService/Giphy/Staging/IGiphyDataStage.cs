using System.Collections.Immutable;
using TrendingGiphyBotWorkerService.Giphy.Staging.GifFinding.Api;

namespace TrendingGiphyBotWorkerService.Giphy.Staging;

public interface IGiphyDataStage
{
    IImmutableDictionary<ulong, GiphyData> GetChannelGiphyPostStage();
    Task RefreshAsync(CancellationToken cancellationToken = default);
    void Evict(ulong channelId);
}
