namespace TrendingGiphyBotWorkerService.Giphy.Staging.GifFinding.Caching;

public record CachingConfig(int CacheCapacity, TimeSpan TimeSpanBetweenRefreshes);
