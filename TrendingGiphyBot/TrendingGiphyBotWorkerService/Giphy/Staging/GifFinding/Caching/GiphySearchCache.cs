using Microsoft.Extensions.Options;
using TrendingGiphyBotWorkerService.Configuration;
using TrendingGiphyBotWorkerService.Giphy.Staging.GifFinding.Api;
using TrendingGiphyBotWorkerService.Giphy.Staging.GifFinding.Caching.Paging;
using TrendingGiphyBotWorkerService.Logging;

namespace TrendingGiphyBotWorkerService.Giphy.Staging.GifFinding.Caching;

[RegisterSingleton]
public class GiphySearchCache
(
    ILogger<GiphySearchCache> _logger,
    IOptionsMonitor<AppConfig> _appConfig,
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

        _giphyDataListHelper.SortToMaxSize(_searchedGiphyDatas[searchTerms], itemsToAdd, _appConfig.CurrentValue.Giphy.Staging.SearchCaching.CacheCapacity);
        _logger.LogGiphySearchCacheCount(_searchedGiphyDatas[searchTerms].Count);
    }

    public GiphyData? GetFirstGif(string searchTerm) => _searchedGiphyDatas[searchTerm].FirstOrDefault();

    public GiphyData? GetFirstUnseenGif(string searchTerm, string[] idsAlreadySeen) => _searchedGiphyDatas[searchTerm].FirstOrDefault(s => !idsAlreadySeen.Contains(s.Id));
}
