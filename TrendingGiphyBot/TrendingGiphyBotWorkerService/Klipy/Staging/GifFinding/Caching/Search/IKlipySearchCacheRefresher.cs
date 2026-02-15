namespace TrendingGiphyBotWorkerService.Klipy.Staging.GifFinding.Caching.Search;

public interface IKlipySearchCacheRefresher
{
    Task RefreshSearchCachesForActiveKeywordsAsync(CancellationToken cancellationToken = default);
}
