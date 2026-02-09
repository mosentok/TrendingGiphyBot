namespace TrendingGiphyBotWorkerService.Giphy.Staging.GifFinding.Caching;

public record GiphyCachingConfig(int CacheCapacity, TimeSpan TimeSpanBetweenRefreshes);
