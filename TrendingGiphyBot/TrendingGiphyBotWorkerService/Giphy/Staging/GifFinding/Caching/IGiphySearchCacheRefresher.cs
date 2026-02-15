namespace TrendingGiphyBotWorkerService.Giphy.Staging.GifFinding.Caching;

public interface IGiphySearchCacheRefresher
{
    Task RefreshSearchCachesForActiveKeywordsAsync(CancellationToken cancellationToken = default);
}
