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
        foreach (var rating in _appConfig.CurrentValue.Giphy.Staging.Ratings)
        {
            var cachedSearchGifs = GetCachedSearchGifs();

            var itemsToAdd = await _giphySearchPager.SearchAsync(searchTerms, rating, cancellationToken);

            _giphyDataListHelper.SortToMaxSize(cachedSearchGifs, itemsToAdd, _appConfig.CurrentValue.Giphy.Staging.SearchCaching.CacheCapacity);
            _logger.LogGiphySearchCacheCount(cachedSearchGifs.Count);

            List<GiphyData> GetCachedSearchGifs()
            {
                var key = $"{searchTerms}:{rating}";

                if (_searchedGiphyDatas.TryGetValue(key, out var cachedSearchGifs))
                    return cachedSearchGifs;

                _searchedGiphyDatas[key] = [];

                return _searchedGiphyDatas[key];
            }
        }
    }

    public GiphyData? GetFirstGif(string searchTerm, string rating) =>
        _searchedGiphyDatas.TryGetValue(BuildKey(searchTerm, rating), out var cachedSearchGifs)
            ? cachedSearchGifs.FirstOrDefault()
            : null;

    public GiphyData? GetFirstUnseenGif(string searchTerm, string[] idsAlreadySeen, string rating) =>
        _searchedGiphyDatas.TryGetValue(BuildKey(searchTerm, rating), out var cachedSearchGifs)
            ? cachedSearchGifs.FirstOrDefault(s => !idsAlreadySeen.Contains(s.Id))
            : null;

    static string BuildKey(string searchTerm, string rating) => $"{searchTerm}:{rating}";
}
