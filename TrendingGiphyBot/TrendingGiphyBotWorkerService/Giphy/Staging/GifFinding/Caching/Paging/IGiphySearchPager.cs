using TrendingGiphyBotWorkerService.Giphy.Staging.GifFinding.Api;

namespace TrendingGiphyBotWorkerService.Giphy.Staging.GifFinding.Caching.Paging;

public interface IGiphySearchPager
{
    Task<List<GiphyData>> SearchAsync(string searchTerms, CancellationToken cancellationToken);
}
