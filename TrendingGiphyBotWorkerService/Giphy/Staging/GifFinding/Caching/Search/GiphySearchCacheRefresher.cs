using TrendingGiphyBotWorkerService.ChannelSettings;

namespace TrendingGiphyBotWorkerService.Giphy.Staging.GifFinding.Caching.Search;

[RegisterSingleton]
public class GiphySearchCacheRefresher
(
    IActiveKeywordsFinder _activeKeywordsFinder,
    IGiphySearchCache _giphySearchCache
) : IGiphySearchCacheRefresher
{
    public async Task RefreshSearchCachesForActiveKeywordsAsync(CancellationToken cancellationToken = default)
    {
        var activeKeywords = await _activeKeywordsFinder.FindActiveKeywordsAsync(cancellationToken);

        foreach (var keyword in activeKeywords)
            await _giphySearchCache.RefreshSearchedGiphyDatasAsync(keyword, cancellationToken);
    }
}
