using Microsoft.Extensions.Options;
using TrendingGiphyBotWorkerService.Configuration;
using TrendingGiphyBotWorkerService.Klipy.Staging.GifFinding.Api;
using TrendingGiphyBotWorkerService.Klipy.Staging.GifFinding.Caching.Paging;

namespace TrendingGiphyBotWorkerService.Klipy.Staging.GifFinding.Caching.Random;

[RegisterSingleton]
public class KlipyRandomCache
(
    ILogger<KlipyRandomCache> _logger,
    IOptionsMonitor<AppConfig> _appConfig,
    IKlipyPager _klipyPager,
    IKlipyClient _klipyClient,
    IKlipyDataListHelper _klipyDataListHelper
) : IKlipyRandomCache
{
    readonly List<KlipyData> _randomKlipyDatas = [];

    public async Task RefreshRandomGifsAsync(CancellationToken cancellationToken = default)
    {
        var itemsToAdd = await _klipyPager.PageAsync(async (page, ct) => await _klipyClient.GetRandomGifsAsync(page, cancellationToken: ct), cancellationToken);

        _klipyDataListHelper.TrimToMaxSize(_randomKlipyDatas, itemsToAdd, _appConfig.CurrentValue.Klipy.Staging.RandomCaching.CacheCapacity);

        _logger.LogKlipyRandomCacheCount(_randomKlipyDatas.Count);
    }

    public KlipyData? GetFirstGif() => _randomKlipyDatas.FirstOrDefault();

    public KlipyData? GetFirstUnseenGif(ulong[] idsAlreadySeen) => _randomKlipyDatas.FirstOrDefault(s => !idsAlreadySeen.Contains(s.Id));
}
