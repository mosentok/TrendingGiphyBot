namespace TrendingGiphyBotWorkerService.Giphy.Staging.GifFinding.Caching.Search;

public interface IGiphySearchCacheRefresher
{
    Task RefreshSearchCachesForActiveKeywordsAsync(CancellationToken cancellationToken = default);
}
