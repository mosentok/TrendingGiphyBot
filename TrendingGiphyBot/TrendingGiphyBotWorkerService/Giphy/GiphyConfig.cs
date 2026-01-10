namespace TrendingGiphyBotWorkerService.Giphy;

public record GiphyConfig(int MaxPageCount, TimeSpan TimeSpanBetweenRefreshes);