using Microsoft.Extensions.Options;
using TrendingGiphyBotWorkerService.Configuration;
using TrendingGiphyBotWorkerService.Giphy.Staging.GifFinding.Api;
using TrendingGiphyBotWorkerService.Giphy.Staging.GifFinding.Caching.Paging;
using TrendingGiphyBotWorkerService.Logging;

namespace TrendingGiphyBotWorkerService.Giphy.Staging.GifFinding.Caching;

[RegisterSingleton]
public class GiphyTrendingCache
(
    ILogger<GiphyTrendingCache> _logger,
    IOptionsMonitor<AppConfig> _appConfig,
    IGiphyDataListHelper _giphyDataListHelper,
    IGiphyTrendingPager _giphyTrendingPager
) : IGiphyTrendingCache
{
    readonly List<GiphyData> _trendingGiphyDatas = [];

    public async Task RefreshTrendingGifsAsync(CancellationToken cancellationToken = default)
    {
        var itemsToAdd = await _giphyTrendingPager.GetTrendingGifsAsync(cancellationToken);

        _giphyDataListHelper.SortToMaxSize(_trendingGiphyDatas, itemsToAdd, _appConfig.CurrentValue.Giphy.Staging.TrendingCaching.CacheCapacity);
        _logger.LogGiphyTrendingCacheCount(_trendingGiphyDatas.Count);
    }

    public GiphyData? GetFirstGif() => _trendingGiphyDatas.FirstOrDefault();

    public GiphyData? GetFirstUnseenGif(string[] idsAlreadySeen) => _trendingGiphyDatas.FirstOrDefault(s => !idsAlreadySeen.Contains(s.Id));
}
