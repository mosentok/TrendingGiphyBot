using Microsoft.Extensions.Options;
using TrendingGiphyBotWorkerService.Configuration;
using TrendingGiphyBotWorkerService.Klipy.Staging.GifFinding.Api;
using TrendingGiphyBotWorkerService.Klipy.Staging.GifFinding.Caching.Paging;
using TrendingGiphyBotWorkerService.Logging;

namespace TrendingGiphyBotWorkerService.Klipy.Staging.GifFinding.Caching;

[RegisterSingleton]
public class KlipySearchCache
(
    ILogger<KlipySearchCache> _logger,
    IOptions<AppConfig> _appConfig,
    IKlipyPager _klipyPager,
    IKlipyClient _klipyClient,
    IKlipyDataListHelper _klipyDataListHelper
) : IKlipySearchCache
{
    readonly Dictionary<string, List<KlipyData>> _trendingKlipyDatas = [];

    public async Task RefreshSearchGifsAsync(string searchTerms, CancellationToken cancellationToken = default)
    {
        var containsSearchTermsKey = _trendingKlipyDatas.ContainsKey(searchTerms);

        if (!containsSearchTermsKey)
            _trendingKlipyDatas.Add(searchTerms, []);

        var itemsToAdd = await _klipyPager.PageAsync(async (page, ct) => await _klipyClient.SearchGifsAsync(searchTerms, page, cancellationToken: ct), cancellationToken);

        _klipyDataListHelper.TrimToMaxSize(_trendingKlipyDatas[searchTerms], itemsToAdd, _appConfig.Value.Giphy.Staging.Caching.CacheCapacity);

        // TODO klipy specific logs
        _logger.LogGifTrendingCacheCount(_trendingKlipyDatas.Count);
    }

    public KlipyData? GetFirstGif(string searchTerm) => _trendingKlipyDatas[searchTerm].FirstOrDefault();

    public KlipyData? GetFirstUnseenGif(string searchTerm, ulong[] idsAlreadySeen) => _trendingKlipyDatas[searchTerm].FirstOrDefault(s => !idsAlreadySeen.Contains(s.Id));
}
