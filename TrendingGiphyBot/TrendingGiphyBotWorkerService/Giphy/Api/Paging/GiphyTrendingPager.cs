using TrendingGiphyBotWorkerService.Paging;

namespace TrendingGiphyBotWorkerService.Giphy.Api.Paging;

public class GiphyTrendingPager(IPager _pager, IGiphyClient _giphyClient) : IGiphyTrendingPager
{
    public async Task<List<GiphyData>> GetTrendingGifsAsync(CancellationToken cancellationToken) =>
        await _pager.PageAsync(
            searchWithOffsetAsync:
                async offset => await _giphyClient.GetTrendingGifsAsync(offset, cancellationToken: cancellationToken).ContinueWith(s => s.Result.Data));
}
