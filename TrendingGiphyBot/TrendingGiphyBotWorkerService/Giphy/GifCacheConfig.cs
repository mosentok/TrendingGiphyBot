namespace TrendingGiphyBotWorkerService.Giphy;

public record GifCacheConfig(List<GiphyData> Items, int MaxCount);