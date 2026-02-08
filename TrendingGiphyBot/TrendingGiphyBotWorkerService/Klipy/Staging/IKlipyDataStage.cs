using System.Collections.Immutable;
using TrendingGiphyBotWorkerService.Klipy.Staging.GifFinding.Api;

namespace TrendingGiphyBotWorkerService.Klipy.Staging;

public interface IKlipyDataStage
{
    IImmutableDictionary<ulong, KlipyData> GetChannelKlipyPostStage();
    Task RefreshAsync(CancellationToken cancellationToken = default);
    void Evict(ulong channelId);
}
