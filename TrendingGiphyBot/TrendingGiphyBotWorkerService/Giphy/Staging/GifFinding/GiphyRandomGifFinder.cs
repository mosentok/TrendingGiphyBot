using Microsoft.Extensions.Options;
using TrendingGiphyBotWorkerService.ChannelSettings;
using TrendingGiphyBotWorkerService.Giphy.Staging.GifFinding.Api;
using TrendingGiphyBotWorkerService.Giphy.Staging.GifFinding.Caching;
using TrendingGiphyBotWorkerService.Results;

namespace TrendingGiphyBotWorkerService.Giphy.Staging.GifFinding;

[RegisterSingleton]
public class GiphyRandomGifFinder
(
    IGiphyRandomCache _giphyRandomCache,
    IOptionsMonitor<AppConfig> _appConfig
) : IGiphyRandomGifFinder
{
    public Maybe<GiphyData> TryGetRandomGif(ChannelSettingsModel channel)
    {
        if (!_appConfig.CurrentValue.Giphy.Staging.EnableRandomGifs)
            return new();

        if (channel.GiphyPosts is null or { Count: 0 })
        {
            var firstGif = _giphyRandomCache.GetFirstGif();

            return firstGif is not null
                ? new(firstGif)
                : new();
        }

        var seenGiphyDataIds = channel.GiphyPosts.Select(s => s.GiphyDataId).ToArray();
        var firstUnseenGif = _giphyRandomCache.GetFirstUnseenGif(seenGiphyDataIds);

        return firstUnseenGif is not null
            ? new(firstUnseenGif)
            : new();
    }
}
