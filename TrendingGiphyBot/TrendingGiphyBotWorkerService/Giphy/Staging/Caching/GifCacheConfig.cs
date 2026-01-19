using TrendingGiphyBotWorkerService.Giphy.Api;

namespace TrendingGiphyBotWorkerService.Giphy.Staging.Caching;

public record GifCacheConfig(List<GiphyData> Items, int MaxCount);