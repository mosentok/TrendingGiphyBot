using Microsoft.Extensions.Options;
using TrendingGiphyBotWorkerService.ChannelSettings;
using TrendingGiphyBotWorkerService.Klipy.Staging.GifFinding.Api;
using TrendingGiphyBotWorkerService.Klipy.Staging.GifFinding.Caching;
using TrendingGiphyBotWorkerService.Results;

namespace TrendingGiphyBotWorkerService.Klipy.Staging.GifFinding;

[RegisterSingleton]
public class KlipyRandomGifFinder
(
    IKlipyRandomCache _klipyRandomCache,
    IOptionsMonitor<AppConfig> _appConfig
) : IKlipyRandomGifFinder
{
    public Maybe<KlipyData> TryGetRandomGif(ChannelSettingsModel channel, CancellationToken cancellationToken)
    {
        if (!_appConfig.CurrentValue.Klipy.Staging.EnableRandomGifs)
            return new();

        if (channel.KlipyPosts.Count == 0)
        {
            var firstGif = _klipyRandomCache.GetFirstGif();

            return firstGif is not null
                ? new(firstGif)
                : new();
        }

        var seenKlipyDataIds = channel.KlipyPosts.Select(s => s.KlipyDataId).ToArray();
        var firstUnseen = _klipyRandomCache.GetFirstUnseenGif(seenKlipyDataIds);

        return firstUnseen is not null
            ? new(firstUnseen)
            : new();
    }
}
