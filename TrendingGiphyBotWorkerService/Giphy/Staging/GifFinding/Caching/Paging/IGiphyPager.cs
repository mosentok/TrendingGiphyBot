using TrendingGiphyBotWorkerService.Giphy.Staging.GifFinding.Api;

namespace TrendingGiphyBotWorkerService.Giphy.Staging.GifFinding.Caching.Paging;

public interface IGiphyPager
{
    Task<List<GiphyData>> PageAsync(SearchWithOffsetAsync searchWithOffsetAsync);
}
