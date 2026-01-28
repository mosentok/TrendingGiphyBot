using TrendingGiphyBotWorkerService.Giphy.Api;

namespace TrendingGiphyBotWorkerService.Paging;

public interface IPager
{
    Task<List<GiphyData>> PageAsync(SearchWithOffsetAsync searchWithOffsetAsync);
}
