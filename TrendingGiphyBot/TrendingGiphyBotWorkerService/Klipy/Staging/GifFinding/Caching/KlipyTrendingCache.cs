using Microsoft.Extensions.Options;
using TrendingGiphyBotWorkerService.Configuration;
using TrendingGiphyBotWorkerService.Klipy.Staging.GifFinding.Api;
using TrendingGiphyBotWorkerService.Klipy.Staging.GifFinding.Caching.Paging;
using TrendingGiphyBotWorkerService.Logging;

namespace TrendingGiphyBotWorkerService.Klipy.Staging.GifFinding.Caching;

[RegisterSingleton]
public class KlipyTrendingCache
(
    ILogger<KlipyTrendingCache> _logger,
    IOptions<AppConfig> _appConfig,
    IKlipyPager _klipyPager,
    IKlipyClient _klipyClient,
    IKlipyDataListHelper _klipyDataListHelper
) : IKlipyTrendingCache
{
    readonly List<KlipyData> _trendingKlipyDatas = [];

    public async Task RefreshTrendingGifsAsync(CancellationToken cancellationToken = default)
    {
        var itemsToAdd = await _klipyPager.PageAsync(async (page, ct) => await _klipyClient.GetTrendingGifsAsync(page, cancellationToken: ct), cancellationToken);

        _klipyDataListHelper.TrimToMaxSize(_trendingKlipyDatas, itemsToAdd, _appConfig.Value.Giphy.Staging.Caching.CacheCapacity);

        // TODO klipy specific logs
        _logger.LogGifTrendingCacheCount(_trendingKlipyDatas.Count);
    }

    public KlipyData? GetFirstGif() => _trendingKlipyDatas.FirstOrDefault();

    public KlipyData? GetFirstUnseenGif(ulong[] idsAlreadySeen) => _trendingKlipyDatas.FirstOrDefault(s => !idsAlreadySeen.Contains(s.Id));
}
