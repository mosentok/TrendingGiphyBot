namespace TrendingGiphyBotWorkerService.Giphy;

public record GiphyCacheWorkerConfig(int MaxPageCount, TimeSpan TimeSpanBetweenRefreshes, int MaxGiphyCacheLoops);