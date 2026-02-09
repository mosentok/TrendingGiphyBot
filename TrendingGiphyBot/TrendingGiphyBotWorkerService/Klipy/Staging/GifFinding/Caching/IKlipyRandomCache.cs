using TrendingGiphyBotWorkerService.Klipy.Staging.GifFinding.Api;

namespace TrendingGiphyBotWorkerService.Klipy.Staging.GifFinding.Caching;

public interface IKlipyRandomCache
{
    KlipyData? GetFirstGif();
    KlipyData? GetFirstUnseenGif(ulong[] idsAlreadySeen);
    Task RefreshRandomGifsAsync(CancellationToken cancellationToken = default);
}