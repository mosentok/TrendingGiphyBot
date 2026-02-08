using TrendingGiphyBotWorkerService.Klipy.Staging.GifFinding.Caching;

namespace TrendingGiphyBotWorkerService.Giphy.Staging;

public record GiphyStagingConfig(KlipyCachingConfig Caching, bool EnableRandomGifs, bool EnableSearchGifs, bool EnableTrendingGifs, int MaxRandomGifAttempts, TimeSpan TimeSpanBetweenRefreshes);
