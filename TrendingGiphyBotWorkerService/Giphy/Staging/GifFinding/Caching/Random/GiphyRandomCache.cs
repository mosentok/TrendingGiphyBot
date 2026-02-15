using Microsoft.Extensions.Options;
using TrendingGiphyBotWorkerService.Configuration;
using TrendingGiphyBotWorkerService.Giphy.Staging.GifFinding.Api;

namespace TrendingGiphyBotWorkerService.Giphy.Staging.GifFinding.Caching.Random;

[RegisterSingleton]
public class GiphyRandomCache
(
    ILogger<GiphyRandomCache> _logger,
    IOptionsMonitor<AppConfig> _appConfig,
    IGiphyClient _giphyClient,
    IGiphyDataListHelper _giphyDataListHelper
) : IGiphyRandomCache
{
    readonly Dictionary<string, List<GiphyData>> _randomGiphyDatasByRating = [];

    public async Task RefreshRandomGifsAsync(CancellationToken cancellationToken = default)
    {
        foreach (var rating in _appConfig.CurrentValue.Giphy.Staging.Ratings)
        {
            var cachedRandomGifs = GetCachedRandomGifs();

            var randomGif = await _giphyClient.GetRandomGifAsync(rating, cancellationToken: cancellationToken);
            var isAlreadyInCache = cachedRandomGifs.Any(existing => existing.Id == randomGif.Data.Id);

            if (!isAlreadyInCache)
                cachedRandomGifs.Add(randomGif.Data);

            _giphyDataListHelper.TrimToMaxSize(cachedRandomGifs, _appConfig.CurrentValue.Giphy.Staging.RandomCaching.CacheCapacity);
            _logger.LogGiphyRandomCacheCount(cachedRandomGifs.Count);

            List<GiphyData> GetCachedRandomGifs()
            {
                if (_randomGiphyDatasByRating.TryGetValue(rating, out var cachedRandomGifs))
                    return cachedRandomGifs;

                _randomGiphyDatasByRating[rating] = [];

                return _randomGiphyDatasByRating[rating];
            }
        }
    }

    public GiphyData? GetFirstGif(string rating) =>
        _randomGiphyDatasByRating.TryGetValue(rating, out var cachedRandomGifs)
            ? cachedRandomGifs.FirstOrDefault()
            : null;

    public GiphyData? GetFirstUnseenGif(string[] idsAlreadySeen, string rating) =>
        _randomGiphyDatasByRating.TryGetValue(rating, out var cachedRandomGifs)
            ? cachedRandomGifs.FirstOrDefault(s => !idsAlreadySeen.Contains(s.Id))
            : null;
}
