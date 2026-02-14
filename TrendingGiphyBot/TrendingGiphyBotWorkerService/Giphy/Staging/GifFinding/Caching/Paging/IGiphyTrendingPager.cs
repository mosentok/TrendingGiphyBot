using TrendingGiphyBotWorkerService.Giphy.Staging.GifFinding.Api;

namespace TrendingGiphyBotWorkerService.Giphy.Staging.GifFinding.Caching.Paging;

public interface IGiphyTrendingPager
{
    Task<List<GiphyData>> GetTrendingGifsAsync(string rating, CancellationToken cancellationToken);
}
