using TrendingGiphyBotWorkerService.Giphy.Staging.GifFinding.Caching;

namespace TrendingGiphyBotWorkerService.Giphy.Staging;

public record GiphyStagingConfig
(
    GiphyCachingConfig SearchCaching,
    GiphyCachingConfig TrendingCaching,
    GiphyCachingConfig RandomCaching,
    bool EnableRandomGifs,
    bool EnableSearchGifs,
    bool EnableTrendingGifs,
    int MaxRandomGifAttempts,
    TimeSpan TimeSpanBetweenRefreshes,
    string[] Ratings
);
