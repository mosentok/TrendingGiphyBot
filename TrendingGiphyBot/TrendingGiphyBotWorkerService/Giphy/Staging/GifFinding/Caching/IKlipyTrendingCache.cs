using TrendingGiphyBotWorkerService.Klipy.Staging.GifFinding.Api;

namespace TrendingKlipyBotWorkerService.Klipy.Staging.GifFinding.Caching;

public interface IKlipyTrendingCache
{
    KlipyData? GetFirstGif();
    KlipyData? GetFirstUnseenGif(ulong[] idsAlreadySeen);
    Task RefreshTrendingGifsAsync(CancellationToken cancellationToken = default);
}