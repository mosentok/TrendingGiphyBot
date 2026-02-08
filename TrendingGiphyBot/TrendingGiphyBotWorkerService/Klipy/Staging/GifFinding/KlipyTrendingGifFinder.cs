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
    IKlipyTrendingCache _giphyTrendingCache,
    IOptions<AppConfig> _appConfig
) : IKlipyTrendingGifFinder
{
    public Maybe<KlipyData> TryGetTrendingGif(ChannelSettingsModel channel)
    {
        if (!_appConfig.Value.Giphy.Staging.EnableTrendingGifs)
            return new();

        if (channel.KlipyPosts.Count == 0)
        {
            var firstGif = _giphyTrendingCache.GetFirstGif();

            return firstGif is not null
                ? new(firstGif)
                : new();
        }

        var seenKlipyDataIds = channel.KlipyPosts.Select(s => s.KlipyDataId).ToArray();
        var firstUnseenGif = _giphyTrendingCache.GetFirstUnseenGif(seenKlipyDataIds);

        if (firstUnseenGif is not null)
            return new(firstUnseenGif);

        return new();
    }
}
