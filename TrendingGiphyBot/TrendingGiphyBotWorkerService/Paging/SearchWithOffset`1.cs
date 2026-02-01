using TrendingGiphyBotWorkerService.Giphy.Staging.GifFinding.Api;

namespace TrendingGiphyBotWorkerService.Paging;

public delegate Task<GiphyResponse> SearchWithOffsetAsync(int offset);
