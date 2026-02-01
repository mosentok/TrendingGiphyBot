namespace TrendingGiphyBotWorkerService.Giphy.Staging.Caching;

public record CachingConfig(int? CacheCapacity, TimeSpan? TimeSpanBetweenRefreshes);
