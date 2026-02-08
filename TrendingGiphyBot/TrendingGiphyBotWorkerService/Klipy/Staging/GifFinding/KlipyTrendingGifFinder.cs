using Microsoft.Extensions.Options;
using TrendingGiphyBotWorkerService.ChannelSettings;
using TrendingGiphyBotWorkerService.GifPostingBehavior;
using TrendingGiphyBotWorkerService.Klipy.Staging.GifFinding.Api;
using TrendingGiphyBotWorkerService.Klipy.Staging.GifFinding.Caching;
using TrendingGiphyBotWorkerService.Results;

namespace TrendingGiphyBotWorkerService.Klipy.Staging.GifFinding;

[RegisterSingleton]
public class KlipyTrendingGifFinder
(
    IKlipyTrendingCache _klipyTrendingCache,
    IOptions<AppConfig> _appConfig
) : IKlipyTrendingGifFinder
{
    public Maybe<KlipyData> TryGetTrendingGif(ChannelSettingsModel channel)
    {
        if (!_appConfig.Value.Klipy.Staging.EnableTrendingGifs)
            return new();

        if (channel.KlipyPosts.Count == 0)
        {
            var firstKlipy = _klipyTrendingCache.GetFirstGif();

            return firstKlipy is not null
                ? new(firstKlipy)
                : new();
        }

        var seenKlipyDataIds = channel.KlipyPosts.Select(s => s.KlipyDataId).ToArray();
        var firstUnseenKlipy = _klipyTrendingCache.GetFirstUnseenGif(seenKlipyDataIds);

        if (firstUnseenKlipy is not null)
            return new(firstUnseenKlipy);

        return new();
    }
}
