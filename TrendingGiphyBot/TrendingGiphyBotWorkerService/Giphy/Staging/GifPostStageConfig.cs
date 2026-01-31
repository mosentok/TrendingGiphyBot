namespace TrendingGiphyBotWorkerService.Giphy.Staging;

public record GifPostStageConfig(int MaxRandomGifAttempts, bool EnableTrendingGifs, bool EnableSearchGifs, bool EnableRandomGifs);
