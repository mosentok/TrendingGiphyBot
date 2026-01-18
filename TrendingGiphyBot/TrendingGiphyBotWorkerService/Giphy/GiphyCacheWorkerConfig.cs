namespace TrendingGiphyBotWorkerService.Giphy;

public record GiphyCacheWorkerConfig(int MaxPageCount, TimeSpan TimeSpanBetweenCacheRefreshes, int MaxGiphyCacheLoops);
