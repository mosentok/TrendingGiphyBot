using TrendingGiphyBotWorkerService.Giphy.Staging.GifFinding.Caching;

namespace TrendingGiphyBotWorkerService.Giphy.Staging;

public record StagingConfig(CachingConfig Caching, bool EnableRandomGifs, bool EnableSearchGifs, bool EnableTrendingGifs, int MaxRandomGifAttempts, TimeSpan TimeSpanBetweenRefreshes);
