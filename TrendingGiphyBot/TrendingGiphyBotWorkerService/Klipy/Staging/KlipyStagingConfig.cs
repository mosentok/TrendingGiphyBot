using TrendingGiphyBotWorkerService.Klipy.Staging.GifFinding.Caching;

namespace TrendingGiphyBotWorkerService.Klipy.Staging;

public record KlipyStagingConfig
(
    KlipyCachingConfig SearchCaching,
    KlipyCachingConfig TrendingCaching,
    KlipyCachingConfig RandomCaching,
    bool EnableRandomGifs,
    bool EnableSearchGifs,
    bool EnableTrendingGifs,
    TimeSpan TimeSpanBetweenRefreshes
);

