using Microsoft.Extensions.Options;
using TrendingGiphyBotWorkerService.ChannelSettings;
using TrendingGiphyBotWorkerService.Giphy.Staging.GifFinding.Api;
using TrendingGiphyBotWorkerService.Giphy.Staging.GifFinding.Caching;

namespace TrendingGiphyBotWorkerService.Giphy.Staging.GifFinding;

[RegisterSingleton]
public class GiphyRandomGifFinder
(
    IGiphyRandomCache _giphyRandomCache,
    IOptionsMonitor<AppConfig> _appConfig
) : IGiphyRandomGifFinder
{
    public GiphyData? TryGetRandomGif(ChannelSettingsModel channel)
    {
        if (!_appConfig.CurrentValue.Giphy.Staging.EnableRandomGifs)
            return null;

        if (channel.GiphyPosts is null or { Count: 0 })
        {
            var firstGif = _giphyRandomCache.GetFirstGif();

            return firstGif is not null
                ? firstGif
                : null;
        }

        var seenGiphyDataIds = channel.GiphyPosts.Select(s => s.GiphyDataId).ToArray();
        var firstUnseenGif = _giphyRandomCache.GetFirstUnseenGif(seenGiphyDataIds);

        return firstUnseenGif is not null
            ? firstUnseenGif
            : null;
    }
}
