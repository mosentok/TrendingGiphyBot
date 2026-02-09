using Microsoft.Extensions.Options;
using TrendingGiphyBotWorkerService.Configuration;
using TrendingGiphyBotWorkerService.Giphy.Staging.GifFinding.Api;
using TrendingGiphyBotWorkerService.Logging;

namespace TrendingGiphyBotWorkerService.Giphy.Staging.GifFinding.Caching;

[RegisterSingleton]
public class GiphyRandomCache
(
    ILogger<GiphyRandomCache> _logger,
    IOptionsMonitor<AppConfig> _appConfig,
    IGiphyClient _giphyClient,
    IGiphyDataListHelper _giphyDataListHelper
) : IGiphyRandomCache
{
    readonly List<GiphyData> _randomGiphyDatas = [];

    public async Task RefreshRandomGifsAsync(CancellationToken cancellationToken = default)
    {
        var randomGif = await _giphyClient.GetRandomGifAsync(cancellationToken: cancellationToken);

        var isAlreadyInCache = _randomGiphyDatas.Any(existing => existing.Id == randomGif.Data.Id);

        if (!isAlreadyInCache)
            _randomGiphyDatas.Add(randomGif.Data);

        _giphyDataListHelper.TrimToMaxSize(_randomGiphyDatas, _appConfig.CurrentValue.Giphy.Staging.RandomCaching.CacheCapacity);
        _logger.LogGiphyRandomCacheCount(_randomGiphyDatas.Count);
    }

    public GiphyData? GetFirstGif() => _randomGiphyDatas.FirstOrDefault();

    public GiphyData? GetFirstUnseenGif(string[] idsAlreadySeen) => _randomGiphyDatas.FirstOrDefault(s => !idsAlreadySeen.Contains(s.Id));
}
