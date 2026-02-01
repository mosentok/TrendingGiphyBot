using TrendingGiphyBotWorkerService.Giphy.Staging.GifFinding.Api;
using TrendingGiphyBotWorkerService.Paging;

namespace TrendingGiphyBotWorkerService.Giphy.Staging.GifFinding.Caching.Paging;

public class GiphySearchPager(IPager _pager, IGiphyClient _giphyClient) : IGiphySearchPager
{
    public async Task<List<GiphyData>> SearchAsync(string searchTerms, CancellationToken cancellationToken) =>
        await _pager.PageAsync(
            searchWithOffsetAsync:
                async offset => await _giphyClient.SearchAsync(searchTerms, offset, cancellationToken: cancellationToken));
}
