using Microsoft.Extensions.Options;
using TrendingGiphyBotWorkerService.Configuration;
using TrendingGiphyBotWorkerService.Klipy.Staging.GifFinding.Api;
using TrendingGiphyBotWorkerService.Klipy.Staging.GifFinding.Caching.Paging;

namespace TrendingGiphyBotWorkerService.Klipy.Staging.GifFinding.Caching;

[RegisterSingleton]
public class KlipyRandomCache
(
    ILogger<KlipySearchCache> _logger,
    IOptions<AppConfig> _appConfig,
    IKlipyPager _klipyPager,
    IKlipyClient _klipyClient,
    IKlipyDataListHelper _klipyDataListHelper
) : IKlipyRandomCache
{
    readonly List<KlipyData> _trendingKlipyDatas = [];

    public async Task RefreshSearchGifsAsync(CancellationToken cancellationToken = default)
    {
        var itemsToAdd = await _klipyPager.PageAsync(async (page, ct) => await _klipyClient.GetRandomGifsAsync(page, cancellationToken: ct), cancellationToken);

        _klipyDataListHelper.TrimToMaxSize(_trendingKlipyDatas, itemsToAdd, _appConfig.Value.Klipy.Staging.Caching.CacheCapacity);

        // TODO this should be a random log
        //_logger.LogKlipyTrendingCacheCount(_trendingKlipyDatas.Count);
    }

    public KlipyData? GetFirstGif() => _trendingKlipyDatas.FirstOrDefault();

    public KlipyData? GetFirstUnseenGif(ulong[] idsAlreadySeen) => _trendingKlipyDatas.FirstOrDefault(s => !idsAlreadySeen.Contains(s.Id));
}
