using Microsoft.Extensions.Options;
using TrendingGiphyBotWorkerService.Giphy.Api;
using TrendingGiphyBotWorkerService.Giphy.Api.Paging;
using TrendingGiphyBotWorkerService.Logging;

namespace TrendingGiphyBotWorkerService.Giphy.Staging.Caching;

public class GiphyTrendingCache
(
    ILogger<GiphyTrendingCache> _logger,
    IOptions<AppConfig> _appConfig,
    IGiphyDataListHelper _giphyDataListHelper,
    IGiphyTrendingPager _giphyTrendingPager
) : IGiphyTrendingCache
{
    readonly List<GiphyData> _trendingGiphyDatas = [];

    public async Task RefreshTrendingGifsAsync(CancellationToken cancellationToken = default)
    {
        var itemsToAdd = await _giphyTrendingPager.GetTrendingGifsAsync(cancellationToken);

        _giphyDataListHelper.SortToMaxSize(_trendingGiphyDatas, itemsToAdd, _appConfig.Value.Giphy.Staging.Caching.CacheCapacity.Value);
        _logger.LogGifTrendingCacheCount(_trendingGiphyDatas.Count);
    }

    public GiphyData? GetFirstGif() => _trendingGiphyDatas.FirstOrDefault();

    public GiphyData? GetFirstUnseenGif(string[] idsAlreadySeen) => _trendingGiphyDatas.FirstOrDefault(s => !idsAlreadySeen.Contains(s.Id));
}
