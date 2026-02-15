namespace TrendingGiphyBotWorkerService.Klipy.Staging.GifFinding.Caching;

public interface IKlipySearchCacheRefresher
{
    Task RefreshSearchCachesForActiveKeywordsAsync(CancellationToken cancellationToken = default);
}
