using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using TrendingGiphyBotWorkerService.ChannelSettings;
using TrendingGiphyBotWorkerService.Configuration;
using TrendingGiphyBotWorkerService.GifPostingBehavior;
using TrendingGiphyBotWorkerService.Giphy.Staging.GifFinding.Api;
using TrendingGiphyBotWorkerService.Giphy.Staging.GifFinding.Caching;

namespace TrendingGiphyBotWorkerService.Giphy.Staging.GifFinding;

[RegisterSingleton]
public class GiphyTrendingGifFinder
(
    IGiphyTrendingCache _giphyTrendingCache,
    IOptionsMonitor<AppConfig> _appConfig
) : IGiphyTrendingGifFinder
{
    public GiphyData? TryGetTrendingGif(ChannelSettingsModel channel)
    {
        if (!_appConfig.CurrentValue.Giphy.Staging.EnableTrendingGifs)
            return null;

        if (channel.GiphyPosts is null or { Count: 0 })
        {
            var firstGif = _giphyTrendingCache.GetFirstGif();

            return firstGif is not null
                ? firstGif
                : null;
        }

        var seenGiphyDataIds = channel.GiphyPosts.Select(s => s.GiphyDataId).ToArray();
        var firstUnseenGif = _giphyTrendingCache.GetFirstUnseenGif(seenGiphyDataIds);

        if (firstUnseenGif is not null)
            return firstUnseenGif;

        return null;
    }
}
