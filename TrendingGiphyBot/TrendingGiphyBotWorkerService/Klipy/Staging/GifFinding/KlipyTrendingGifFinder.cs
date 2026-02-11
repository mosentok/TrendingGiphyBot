using Microsoft.Extensions.Options;
using TrendingGiphyBotWorkerService.ChannelSettings;
using TrendingGiphyBotWorkerService.GifPostingBehavior;
using TrendingGiphyBotWorkerService.Klipy.Staging.GifFinding.Api;
using TrendingGiphyBotWorkerService.Klipy.Staging.GifFinding.Caching;

namespace TrendingGiphyBotWorkerService.Klipy.Staging.GifFinding;

[RegisterSingleton]
public class KlipyTrendingGifFinder
(
    IKlipyTrendingCache _giphyTrendingCache,
    IOptionsMonitor<AppConfig> _appConfig
) : IKlipyTrendingGifFinder
{
    public KlipyData? TryGetTrendingGif(ChannelSettingsModel channel)
    {
        if (!_appConfig.CurrentValue.Klipy.Staging.EnableTrendingGifs)
            return null;

        if (channel.KlipyPosts.Count == 0)
        {
            var firstGif = _giphyTrendingCache.GetFirstGif();

            return firstGif is not null
                ? firstGif
                : null;
        }

        var seenKlipyDataIds = channel.KlipyPosts.Select(s => s.KlipyDataId).ToArray();
        var firstUnseenGif = _giphyTrendingCache.GetFirstUnseenGif(seenKlipyDataIds);

        if (firstUnseenGif is not null)
            return firstUnseenGif;

        return null;
    }
}
