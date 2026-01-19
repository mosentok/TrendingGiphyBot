namespace TrendingGiphyBotWorkerService.Giphy.Staging.Caching;

public record GiphyCacheWorkerConfig(int MaxPageCount, TimeSpan TimeSpanBetweenCacheRefreshes, int MaxGiphyCacheLoops);
