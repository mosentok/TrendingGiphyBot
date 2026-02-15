using Microsoft.Extensions.Options;
using TrendingGiphyBotWorkerService.Configuration;
using TrendingGiphyBotWorkerService.Giphy.Staging.GifFinding.Api;
using TrendingGiphyBotWorkerService.Giphy.Staging.GifFinding.Caching.Paging;

namespace TrendingGiphyBotWorkerService.Giphy.Staging.GifFinding.Caching.Trending;

[RegisterSingleton]
public class GiphyTrendingCache
(
    ILogger<GiphyTrendingCache> _logger,
    IOptionsMonitor<AppConfig> _appConfig,
    IGiphyDataListHelper _giphyDataListHelper,
    IGiphyTrendingPager _giphyTrendingPager
) : IGiphyTrendingCache
{
    readonly Dictionary<string, List<GiphyData>> _trendingGiphyDatasByRating = [];

    public async Task RefreshTrendingGifsAsync(CancellationToken cancellationToken = default)
    {
        foreach (var rating in _appConfig.CurrentValue.Giphy.Staging.Ratings)
        {
            var cachedTrendingGifs = GetCachedTrendingGifs();

            var itemsToAdd = await _giphyTrendingPager.GetTrendingGifsAsync(rating, cancellationToken);

            _giphyDataListHelper.SortToMaxSize(cachedTrendingGifs, itemsToAdd, _appConfig.CurrentValue.Giphy.Staging.TrendingCaching.CacheCapacity);
            _logger.LogGiphyTrendingCacheCount(cachedTrendingGifs.Count);

            List<GiphyData> GetCachedTrendingGifs()
            {
                if (_trendingGiphyDatasByRating.TryGetValue(rating, out var cachedTrendingGifs))
                    return cachedTrendingGifs;

                _trendingGiphyDatasByRating[rating] = [];

                return _trendingGiphyDatasByRating[rating];
            }
        }
    }

    public GiphyData? GetFirstGif(string rating) =>
        _trendingGiphyDatasByRating.TryGetValue(rating, out var cachedTrendingGifs)
            ? cachedTrendingGifs.FirstOrDefault()
            : null;

    public GiphyData? GetFirstUnseenGif(string[] idsAlreadySeen, string rating) =>
        _trendingGiphyDatasByRating.TryGetValue(rating, out var cachedTrendingGifs)
            ? cachedTrendingGifs.FirstOrDefault(s => !idsAlreadySeen.Contains(s.Id))
            : null;
}
