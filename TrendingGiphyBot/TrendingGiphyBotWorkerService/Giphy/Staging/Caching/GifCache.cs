using TrendingGiphyBotWorkerService.Giphy.Api;
using TrendingGiphyBotWorkerService.Logging;

namespace TrendingGiphyBotWorkerService.Giphy.Staging.Caching;

public class GifCache(
    ILogger<GifCache> _logger,
    GifCacheConfig _gifCacheConfig,
    IGiphyClient _giphyClient
) : IGifCache
{
    readonly List<GiphyData> _items = [.. _gifCacheConfig.Items.OrderByDescending(s => s.TrendingDatetime)];

    public async Task RefreshAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogGifCacheIsRefreshing();

        try
        {
            var numberOfResponses = 0;

            for (var numberOfLoops = 0; numberOfLoops < _gifCacheConfig.MaxGiphyCacheLoops && numberOfResponses % _gifCacheConfig.MaxPageCount == 0; numberOfLoops++)
            {
                var giphyResponse = await _giphyClient.GetTrendingGifsAsync(offset: numberOfResponses, cancellationToken: cancellationToken);

                var notInListYet = giphyResponse.Data.Where(s => !_items.Contains(s)).ToArray();

                _items.AddRange(notInListYet);
                _items.Sort((left, right) => string.Compare(left.TrendingDatetime, right.TrendingDatetime));

                _logger.LogGifCacheCount(notInListYet.Length);

                numberOfResponses += giphyResponse.Data.Count;
            }

            if (_items.Count > _gifCacheConfig.MaxCount)
            {
                var excessCount = _items.Count - _gifCacheConfig.MaxCount;

                _items.RemoveRange(_gifCacheConfig.MaxCount, excessCount);
            }

            _logger.LogGifCacheHasRefreshed(_items.Count);

        }
        catch (Exception ex)
        {
            _logger.LogGifCacheRefreshException(ex);
        }
    }

    public GiphyData? GetFirstGif() => _items.FirstOrDefault();

    public GiphyData? GetFirstUnseenGif(string[] idsAlreadySeen) => _items.FirstOrDefault(s => !idsAlreadySeen.Contains(s.Id));
}
