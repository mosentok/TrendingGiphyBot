using TrendingGiphyBotWorkerService.Klipy.Staging.GifFinding.Caching;

namespace TrendingGiphyBotWorkerService.Klipy.Staging;

public record KlipyStagingConfig(KlipyCachingConfig Caching, bool EnableRandomGifs, bool EnableSearchGifs, bool EnableTrendingGifs, TimeSpan TimeSpanBetweenRefreshes);

