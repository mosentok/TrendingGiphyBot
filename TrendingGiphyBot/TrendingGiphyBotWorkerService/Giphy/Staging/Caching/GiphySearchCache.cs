using TrendingGiphyBotWorkerService.Giphy.Api;
using TrendingGiphyBotWorkerService.Giphy.Api.Paging;
using TrendingGiphyBotWorkerService.Logging;

namespace TrendingGiphyBotWorkerService.Giphy.Staging.Caching;

public class GiphySearchCache
(
    ILogger<GiphySearchCache> _logger,
    GifCacheConfig _gifCacheConfig,
    IGiphyDataListHelper _giphyDataListHelper,
    IGiphySearchPager _giphySearchPager
) : IGiphySearchCache
{
    readonly Dictionary<string, List<GiphyData>> _searchedGiphyDatas = [];

    public async Task RefreshSearchedGiphyDatasAsync(string searchTerms, CancellationToken cancellationToken = default)
    {
        var containsSearchTermsKey = _searchedGiphyDatas.ContainsKey(searchTerms);

        if (!containsSearchTermsKey)
            _searchedGiphyDatas.Add(searchTerms, []);

        var itemsToAdd = await _giphySearchPager.SearchAsync(searchTerms, cancellationToken);

        _giphyDataListHelper.SortToMaxSize(_searchedGiphyDatas[searchTerms], itemsToAdd, _gifCacheConfig.MaxCount);
        _logger.LogGifSearchCacheCount(_searchedGiphyDatas[searchTerms].Count);
    }

    public GiphyData? GetFirstUnseenGif(string searchTerm, string[] idsAlreadySeen) => _searchedGiphyDatas[searchTerm].FirstOrDefault(s => !idsAlreadySeen.Contains(s.Id));
}
