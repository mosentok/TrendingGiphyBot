using Microsoft.Extensions.Options;
using TrendingGiphyBotWorkerService.ChannelSettings;
using TrendingGiphyBotWorkerService.Klipy.Staging.GifFinding.Api;
using TrendingGiphyBotWorkerService.Klipy.Staging.GifFinding.Caching.Random;

namespace TrendingGiphyBotWorkerService.Klipy.Staging.GifFinding;

[RegisterSingleton]
public class KlipyRandomGifFinder
(
    IKlipyRandomCache _klipyRandomCache,
    IOptionsMonitor<AppConfig> _appConfig
) : IKlipyRandomGifFinder
{
    public KlipyData? TryGetRandomGif(ChannelSettingsModel channel)
    {
        if (!_appConfig.CurrentValue.Klipy.Staging.EnableRandomGifs)
            return null;

        if (channel.KlipyPosts.Count == 0)
        {
            var firstGif = _klipyRandomCache.GetFirstGif();

            return firstGif is not null
                ? firstGif
                : null;
        }

        var seenKlipyDataIds = channel.KlipyPosts.Select(s => s.KlipyDataId).ToArray();
        var firstUnseen = _klipyRandomCache.GetFirstUnseenGif(seenKlipyDataIds);

        return firstUnseen is not null
            ? firstUnseen
            : null;
    }
}
