using TrendingGiphyBotWorkerService.Giphy.Staging.GifFinding.Api;

namespace TrendingGiphyBotWorkerService.Paging;

public interface IPager
{
    Task<List<GiphyData>> PageAsync(SearchWithOffsetAsync searchWithOffsetAsync);
}
