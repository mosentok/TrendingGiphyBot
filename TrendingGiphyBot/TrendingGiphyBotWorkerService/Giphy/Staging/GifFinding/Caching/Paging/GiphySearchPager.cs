using TrendingGiphyBotWorkerService.Giphy.Staging.GifFinding.Api;

namespace TrendingGiphyBotWorkerService.Giphy.Staging.GifFinding.Caching.Paging;

[RegisterSingleton]
public class GiphySearchPager(
    IGiphyPager _pager,
    IGiphyClient _giphyClient
) : IGiphySearchPager
{
    public async Task<List<GiphyData>> SearchAsync(string searchTerms, CancellationToken cancellationToken) =>
        await _pager.PageAsync(
            searchWithOffsetAsync:
                async offset => await _giphyClient.SearchAsync(searchTerms, offset, cancellationToken: cancellationToken));
}
