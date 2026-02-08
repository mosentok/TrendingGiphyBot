using TrendingGiphyBotWorkerService.Klipy.Staging.GifFinding.Api;

namespace TrendingGiphyBotWorkerService.Klipy.Staging.GifFinding.Caching;

public interface IKlipySearchCache
{
    KlipyData? GetFirstGif(string searchTerm);
    KlipyData? GetFirstUnseenGif(string searchTerm, ulong[] idsAlreadySeen);
    Task RefreshSearchGifsAsync(string searchTerms, CancellationToken cancellationToken = default);
}