using TrendingGiphyBotWorkerService.Giphy.Api;

namespace TrendingGiphyBotWorkerService.Paging;

public delegate Task<GiphyResponse> SearchWithOffsetAsync(int offset);
