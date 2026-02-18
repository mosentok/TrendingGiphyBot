using Microsoft.Extensions.Options;
using TrendingGiphyBotWorkerService.Configuration;
using TrendingGiphyBotWorkerService.Klipy.Staging.GifFinding.Api;
using TrendingGiphyBotWorkerService.Klipy.Staging.GifFinding.Caching.Paging;

namespace TrendingGiphyBotWorkerService.Klipy.Staging.GifFinding.Caching.Search;

[RegisterSingleton]
public class KlipySearchCache
(
    ILogger<KlipySearchCache> _logger,
    IOptionsMonitor<AppConfig> _appConfig,
    IKlipyPager _klipyPager,
    IKlipyClient _klipyClient,
    IKlipyDataListHelper _klipyDataListHelper
) : IKlipySearchCache
{
    readonly Dictionary<string, List<KlipyData>> _searchedKlipyDatas = new();

    public async Task RefreshSearchGifsAsync(string searchTerms, CancellationToken cancellationToken = default)
    {
        var containsSearchTermsKey = _searchedKlipyDatas.ContainsKey(searchTerms);

        if (!containsSearchTermsKey)
            _searchedKlipyDatas.Add(searchTerms, []);

        var itemsToAdd = await _klipyPager.PageAsync(async (page, ct) => await _klipyClient.SearchGifsAsync(searchTerms, page, cancellationToken: ct), cancellationToken);

        _klipyDataListHelper.TrimToMaxSize(_searchedKlipyDatas[searchTerms], itemsToAdd, _appConfig.CurrentValue.Klipy.Staging.SearchCaching.CacheCapacity);

        _logger.LogKlipySearchCacheCount(_searchedKlipyDatas[searchTerms].Count);
    }

    public KlipyData? GetFirstGif(string searchTerm) =>
        _searchedKlipyDatas.TryGetValue(searchTerm, out var result)
            ? result.FirstOrDefault()
            : null;

    public KlipyData? GetFirstUnseenGif(string searchTerm, ulong[] idsAlreadySeen) =>
        _searchedKlipyDatas.TryGetValue(searchTerm, out var result)
            ? result.FirstOrDefault(s => !idsAlreadySeen.Contains(s.Id))
            : null;
}
