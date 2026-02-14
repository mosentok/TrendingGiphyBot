using TrendingGiphyBotWorkerService.Giphy.Staging.GifFinding.Api;

namespace TrendingGiphyBotWorkerService.Giphy.Staging.GifFinding.Caching.Paging;

[RegisterSingleton]
public class GiphyTrendingPager(
    IGiphyPager _giphyPager,
    IGiphyClient _giphyClient
) : IGiphyTrendingPager
{
    public async Task<List<GiphyData>> GetTrendingGifsAsync(string rating, CancellationToken cancellationToken) =>
        await _giphyPager.PageAsync(
            searchWithOffsetAsync:
                async offset => await _giphyClient.GetTrendingGifsAsync(rating, offset, cancellationToken: cancellationToken));
}
