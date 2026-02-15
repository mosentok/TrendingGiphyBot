using TrendingGiphyBotWorkerService.ChannelSettings;

namespace TrendingGiphyBotWorkerService.Klipy.Staging.GifFinding.Caching.Search;

[RegisterSingleton]
public class KlipySearchCacheRefresher
(
    IActiveKeywordsFinder _activeKeywordsFinder,
    IKlipySearchCache _klipySearchCache
) : IKlipySearchCacheRefresher
{
    public async Task RefreshSearchCachesForActiveKeywordsAsync(CancellationToken cancellationToken = default)
    {
        var activeKeywords = await _activeKeywordsFinder.FindActiveKeywordsAsync(cancellationToken);

        foreach (var keyword in activeKeywords)
            await _klipySearchCache.RefreshSearchGifsAsync(keyword, cancellationToken);
    }
}
