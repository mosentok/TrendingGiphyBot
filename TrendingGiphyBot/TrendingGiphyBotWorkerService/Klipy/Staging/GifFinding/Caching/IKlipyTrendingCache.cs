using TrendingGiphyBotWorkerService.Klipy.Staging.GifFinding.Api;

namespace TrendingGiphyBotWorkerService.Klipy.Staging.GifFinding.Caching;

public interface IKlipyTrendingCache
{
    KlipyData? GetFirstGif();
    KlipyData? GetFirstUnseenGif(ulong[] idsAlreadySeen);
    Task RefreshTrendingGifsAsync(CancellationToken cancellationToken = default);
}