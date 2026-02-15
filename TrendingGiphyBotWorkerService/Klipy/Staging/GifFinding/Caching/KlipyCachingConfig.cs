namespace TrendingGiphyBotWorkerService.Klipy.Staging.GifFinding.Caching;

public record KlipyCachingConfig(int CacheCapacity, TimeSpan TimeSpanBetweenRefreshes);
