using System.Collections.Immutable;
using TrendingGiphyBotWorkerService.Klipy.Staging.GifFinding.Api;

namespace TrendingGiphyBotWorkerService.Klipy.Staging;

public interface IKlipyDataStage
{
    IImmutableDictionary<ulong, KlipyData> GetTrendingKlipyPostStage();
    IImmutableDictionary<ulong, KlipyData> GetSearchKlipyPostStage();
    IImmutableDictionary<ulong, KlipyData> GetRandomKlipyPostStage();
    Task RefreshAsync(CancellationToken cancellationToken = default);
    void EvictTrending(ulong channelId);
    void EvictSearch(ulong channelId);
    void EvictRandom(ulong channelId);
}
