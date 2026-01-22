using TrendingGiphyBotWorkerService.Giphy.Api;
using TrendingGiphyBotWorkerService.Logging;

namespace TrendingGiphyBotWorkerService.Giphy.Staging.Caching;

// TODO get rid of _items? unless we expect to seed the values somehow. that ought to be done post-DI by the worker already anyway tho
public class GifCache(
    ILogger<IGifCache> _logger,
    GifCacheConfig _gifCacheConfig,
    IGiphyClient _giphyClient) : IGifCache
{
    public List<GiphyData> Items { get; } = [.. _gifCacheConfig.Items.OrderByDescending(s => s.TrendingDatetime)];

    public async Task RefreshAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var numberOfResponses = 0;

            for (var numberOfLoops = 0; numberOfLoops < _gifCacheConfig.MaxGiphyCacheLoops && numberOfResponses % _gifCacheConfig.MaxPageCount == 0; numberOfLoops++)
            {
                var giphyResponse = await _giphyClient.GetTrendingGifsAsync(offset: numberOfResponses, cancellationToken: cancellationToken);

                var notInListYet = giphyResponse.Data.Where(s => !Items.Contains(s));

                Items.AddRange(notInListYet);
                Items.Sort((left, right) => string.Compare(left.TrendingDatetime, right.TrendingDatetime));

                if (Items.Count <= _gifCacheConfig.MaxCount)
                    return;

                var excessCount = Items.Count - _gifCacheConfig.MaxCount;

                Items.RemoveRange(_gifCacheConfig.MaxCount, excessCount);

                numberOfResponses += giphyResponse.Data.Count;
            }
        }
        catch (Exception ex)
        {
            _logger.LogGifCacheRefreshException(ex);
        }
    }

    public GiphyData? GetFirstUnseenGif() => Items.FirstOrDefault();

    public GiphyData? GetFirstUnseenGif(ICollection<string> idsAlreadySeen) => Items.FirstOrDefault(s => !idsAlreadySeen.Contains(s.Id));
}
